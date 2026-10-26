
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TransactionsIngest.Api.Data;
using TransactionsIngest.Api.Models;
using TransactionsIngest.Api.SourceProviders;

namespace TransactionsIngest.Api.Services;

public class TransactionImportService
{
    private readonly AppDbContext _dbContext;
    private readonly IEnumerable<ISourceTransactionProvider> _sourceProviders;
    private readonly ILogger<TransactionImportService> _logger;

    public TransactionImportService(
        AppDbContext dbContext,
        IEnumerable<ISourceTransactionProvider> sourceProviders,
        ILogger<TransactionImportService> logger)
    {
        _dbContext = dbContext;
        _sourceProviders = sourceProviders;
        _logger = logger;
    }

    public async Task ImportAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshots = new List<(
            string SourceSystem,
            IReadOnlyList<SourceTransaction> Transactions)>();

        // Retrieve all source snapshots before changing the database.
        foreach (var provider in _sourceProviders)
        {
            _logger.LogInformation(
                "Fetching transactions from {SourceSystem}",
                provider.SourceSystem);

            var transactions = await provider.GetTransactionsAsync(
                cancellationToken);

            snapshots.Add((provider.SourceSystem, transactions));

            _logger.LogInformation(
                "Fetched {Count} transactions from {SourceSystem}",
                transactions.Count,
                provider.SourceSystem);
        }

        var now = DateTime.UtcNow;

        // Insert new transactions or update existing transactions.
        foreach (var snapshot in snapshots)
        {
            foreach (var incoming in snapshot.Transactions)
            {
                var existing = await _dbContext.Transactions
                    .SingleOrDefaultAsync(
                        t => t.SourceSystem == snapshot.SourceSystem
                            && t.TransactionId == incoming.TransactionId,
                        cancellationToken);

                if (existing is null)
                {
                    var incomingTimeUtc = NormalizeUtc(
                        incoming.TransactionTime);

                    // Finalize historical transactions on first import.
                    var shouldFinalize =
                        now > incomingTimeUtc.AddHours(24);

                    var transaction = new TransactionRecord
                    {
                        SourceSystem = snapshot.SourceSystem,
                        TransactionId = incoming.TransactionId,
                        CardNumber = incoming.CardNumber,
                        LocationCode = incoming.LocationCode,
                        ProductName = incoming.ProductName,
                        Amount = incoming.Amount,
                        TransactionTime = incomingTimeUtc,
                        Status = shouldFinalize ? "Finalized" : "Active",
                        FirstSeenAtUtc = now,
                        LastSeenAtUtc = now
                    };

                    _dbContext.Transactions.Add(transaction);

                    if (shouldFinalize)
                    {
                        AddAudit(
                            transaction,
                            nameof(TransactionRecord.Status),
                            "Active",
                            "Finalized",
                            now);
                    }

                    continue;
                }

                // Finalized transactions cannot be modified.
                if (existing.Status == "Finalized")
                {
                    continue;
                }

                var transactionTimeUtc = NormalizeUtc(
                    existing.TransactionTime);

                var windowEnd = transactionTimeUtc.AddHours(24);

                // Finalize older active transactions.
                if (existing.Status == "Active" && now > windowEnd)
                {
                    FinalizeTransaction(existing, now);
                    continue;
                }

                // Reactivate a revoked transaction only within
                // its original 24-hour window.
                if (existing.Status == "Revoked")
                {
                    if (now < transactionTimeUtc || now > windowEnd)
                    {
                        continue;
                    }

                    AddAudit(
                        existing,
                        nameof(TransactionRecord.Status),
                        "Revoked",
                        "Active",
                        now);

                    existing.Status = "Active";
                    existing.RevokedAtUtc = null;

                    _logger.LogInformation(
                        "Reactivated transaction {TransactionId} from {SourceSystem}",
                        existing.TransactionId,
                        existing.SourceSystem);
                }

                // Audit and update Amount.
                if (existing.Amount != incoming.Amount)
                {
                    AddAudit(
                        existing,
                        nameof(TransactionRecord.Amount),
                        existing.Amount.ToString(
                            "F2", CultureInfo.InvariantCulture),
                        incoming.Amount.ToString(
                            "F2", CultureInfo.InvariantCulture),
                        now);

                    existing.Amount = incoming.Amount;
                }

                // Audit and update CardNumber.
                if (existing.CardNumber != incoming.CardNumber)
                {
                    AddAudit(
                        existing,
                        nameof(TransactionRecord.CardNumber),
                        existing.CardNumber,
                        incoming.CardNumber,
                        now);

                    existing.CardNumber = incoming.CardNumber;
                }

                // Audit and update LocationCode.
                if (existing.LocationCode != incoming.LocationCode)
                {
                    AddAudit(
                        existing,
                        nameof(TransactionRecord.LocationCode),
                        existing.LocationCode,
                        incoming.LocationCode,
                        now);

                    existing.LocationCode = incoming.LocationCode;
                }

                // Audit and update ProductName.
                if (existing.ProductName != incoming.ProductName)
                {
                    AddAudit(
                        existing,
                        nameof(TransactionRecord.ProductName),
                        existing.ProductName,
                        incoming.ProductName,
                        now);

                    existing.ProductName = incoming.ProductName;
                }

                // Audit and update TransactionTime.
                var incomingTime = NormalizeUtc(
                    incoming.TransactionTime);

                if (transactionTimeUtc != incomingTime)
                {
                    AddAudit(
                        existing,
                        nameof(TransactionRecord.TransactionTime),
                        transactionTimeUtc.ToString(
                            "O", CultureInfo.InvariantCulture),
                        incomingTime.ToString(
                            "O", CultureInfo.InvariantCulture),
                        now);

                    existing.TransactionTime = incomingTime;
                }
            }
        }

        // Detect transactions missing from source snapshots.
        foreach (var snapshot in snapshots)
        {
            var incomingIds = snapshot.Transactions
                .Select(t => t.TransactionId)
                .ToHashSet();

            var activeTransactions = await _dbContext.Transactions
                .Where(t => t.SourceSystem == snapshot.SourceSystem
                    && t.Status == "Active")
                .ToListAsync(cancellationToken);

            foreach (var existing in activeTransactions)
            {
                var transactionTimeUtc = NormalizeUtc(
                    existing.TransactionTime);

                var windowEnd = transactionTimeUtc.AddHours(24);

                // Finalize transactions beyond the 24-hour window.
                if (now > windowEnd)
                {
                    FinalizeTransaction(existing, now);
                    continue;
                }

                // Transactions present in the source are not revoked.
                if (incomingIds.Contains(existing.TransactionId))
                {
                    continue;
                }

                // Revoke only during the 24-hour window.
                if (now < transactionTimeUtc || now > windowEnd)
                {
                    continue;
                }

                AddAudit(
                    existing,
                    nameof(TransactionRecord.Status),
                    "Active",
                    "Revoked",
                    now);

                existing.Status = "Revoked";
                existing.RevokedAtUtc = now;

                _logger.LogInformation(
                    "Revoked transaction {TransactionId} from {SourceSystem}",
                    existing.TransactionId,
                    existing.SourceSystem);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Source transaction times are UTC.
    // SQLite can return DateTime values with Kind = Unspecified.
    // In that case, preserve the stored clock time and mark it UTC.
    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private void AddAudit(
        TransactionRecord transaction,
        string fieldName,
        string? oldValue,
        string? newValue,
        DateTime changedAtUtc)
    {
        _dbContext.TransactionAudits.Add(new TransactionAudit
        {
            TransactionRecord = transaction,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAtUtc = changedAtUtc
        });
    }

    private void FinalizeTransaction(
        TransactionRecord transaction,
        DateTime now)
    {
        if (transaction.Status != "Active")
        {
            return;
        }

        AddAudit(
            transaction,
            nameof(TransactionRecord.Status),
            "Active",
            "Finalized",
            now);

        transaction.Status = "Finalized";

        _logger.LogInformation(
            "Finalized transaction {TransactionId} from {SourceSystem}",
            transaction.TransactionId,
            transaction.SourceSystem);
    }
}
