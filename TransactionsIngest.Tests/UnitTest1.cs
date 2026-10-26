
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransactionsIngest.Api.Data;
using TransactionsIngest.Api.Models;
using TransactionsIngest.Api.Services;
using TransactionsIngest.Api.SourceProviders;
using Xunit;

namespace TransactionsIngest.Tests;

public class UnitTest1
{
    private static SourceTransaction CreateTransaction(
        int transactionId = 100001,
        decimal amount = 24.99m,
        DateTime? transactionTime = null)
    {
        return new SourceTransaction
        {
            TransactionId = transactionId,
            CardNumber = "***1111",
            LocationCode = "UPTOWN",
            ProductName = "Wireless Mouse",
            Amount = amount,
            TransactionTime = transactionTime
                ?? DateTime.UtcNow.AddHours(-1)
        };
    }

    private static async Task<TestDatabase> CreateDatabaseAsync()
    {
        var connection = new SqliteConnection(
            "Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);

        await context.Database.EnsureCreatedAsync();

        return new TestDatabase(connection, context);
    }

    private static TransactionImportService CreateService(
        AppDbContext context,
        params ISourceTransactionProvider[] providers)
    {
        return new TransactionImportService(
            context,
            providers,
            NullLogger<TransactionImportService>.Instance);
    }

    [Fact]
    public async Task ImportAsync_InsertsNewTransaction()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        source.Transactions.Add(CreateTransaction());

        var service = CreateService(database.Context, source);

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal("Store Uptown", transaction.SourceSystem);
        Assert.Equal(100001, transaction.TransactionId);
        Assert.Equal("***1111", transaction.CardNumber);
        Assert.Equal("UPTOWN", transaction.LocationCode);
        Assert.Equal("Wireless Mouse", transaction.ProductName);
        Assert.Equal(24.99m, transaction.Amount);
        Assert.Equal("Active", transaction.Status);

        Assert.Empty(
            await database.Context.TransactionAudits.ToListAsync());
    }

    [Fact]
    public async Task ImportAsync_DoesNotDuplicateTransactions()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        source.Transactions.Add(CreateTransaction());

        var service = CreateService(database.Context, source);

        await service.ImportAsync();
        await service.ImportAsync();

        Assert.Equal(
            1,
            await database.Context.Transactions.CountAsync());

        Assert.Equal(
            0,
            await database.Context.TransactionAudits.CountAsync());
    }

    [Fact]
    public async Task ImportAsync_AllowsSameIdFromDifferentSources()
    {
        await using var database = await CreateDatabaseAsync();

        var uptown = new FakeSourceTransactionProvider(
            "Store Uptown");

        var online = new FakeSourceTransactionProvider(
            "Online");

        uptown.Transactions.Add(CreateTransaction());
        online.Transactions.Add(CreateTransaction());

        var service = CreateService(
            database.Context,
            uptown,
            online);

        await service.ImportAsync();

        Assert.Equal(
            2,
            await database.Context.Transactions.CountAsync());

        Assert.Equal(
            2,
            await database.Context.Transactions
                .Select(t => t.SourceSystem)
                .Distinct()
                .CountAsync());
    }

    [Fact]
    public async Task ImportAsync_UpdatesAmountAndCreatesAudit()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        var transactionTime = DateTime.UtcNow.AddHours(-1);

        source.Transactions.Add(CreateTransaction(
            amount: 24.99m,
            transactionTime: transactionTime));

        var service = CreateService(database.Context, source);

        await service.ImportAsync();

        source.Transactions.Clear();

        source.Transactions.Add(CreateTransaction(
            amount: 39.99m,
            transactionTime: transactionTime));

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal(39.99m, transaction.Amount);

        var audit = await database.Context.TransactionAudits
            .SingleAsync();

        Assert.Equal("Amount", audit.FieldName);
        Assert.Equal("24.99", audit.OldValue);
        Assert.Equal("39.99", audit.NewValue);

        // Repeating the same import must not add another audit.
        await service.ImportAsync();

        Assert.Equal(
            1,
            await database.Context.TransactionAudits.CountAsync());
    }

    [Fact]
    public async Task ImportAsync_RevokesMissingRecentTransaction()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        source.Transactions.Add(CreateTransaction());

        var service = CreateService(database.Context, source);

        await service.ImportAsync();

        // Simulate the transaction disappearing from the source.
        source.Transactions.Clear();

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal("Revoked", transaction.Status);
        Assert.NotNull(transaction.RevokedAtUtc);

        var audit = await database.Context.TransactionAudits
            .SingleAsync();

        Assert.Equal("Status", audit.FieldName);
        Assert.Equal("Active", audit.OldValue);
        Assert.Equal("Revoked", audit.NewValue);

        // A repeated import must not create another audit.
        await service.ImportAsync();

        Assert.Equal(
            1,
            await database.Context.TransactionAudits.CountAsync());
    }

    [Fact]
    public async Task ImportAsync_FinalizesOldTransaction()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        source.Transactions.Add(CreateTransaction(
            transactionTime: DateTime.UtcNow.AddHours(-25)));

        var service = CreateService(database.Context, source);

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal("Finalized", transaction.Status);

        var audit = await database.Context.TransactionAudits
            .SingleAsync();

        Assert.Equal("Status", audit.FieldName);
        Assert.Equal("Active", audit.OldValue);
        Assert.Equal("Finalized", audit.NewValue);
    }

    [Fact]
    public async Task ImportAsync_DoesNotModifyFinalizedTransaction()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        var oldTime = DateTime.UtcNow.AddHours(-25);

        source.Transactions.Add(CreateTransaction(
            amount: 24.99m,
            transactionTime: oldTime));

        var service = CreateService(database.Context, source);

        await service.ImportAsync();

        // Change the amount after the transaction is finalized.
        source.Transactions.Clear();

        source.Transactions.Add(CreateTransaction(
            amount: 99.99m,
            transactionTime: oldTime));

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal("Finalized", transaction.Status);
        Assert.Equal(24.99m, transaction.Amount);

        // Only the original finalization audit should exist.
        Assert.Equal(
            1,
            await database.Context.TransactionAudits.CountAsync());
    }

    [Fact]
    public async Task ImportAsync_ReactivatesRevokedTransaction()
    {
        await using var database = await CreateDatabaseAsync();

        var source = new FakeSourceTransactionProvider(
            "Store Uptown");

        var transactionTime = DateTime.UtcNow.AddHours(-1);

        source.Transactions.Add(CreateTransaction(
            transactionTime: transactionTime));

        var service = CreateService(database.Context, source);

        // First import: Active.
        await service.ImportAsync();

        // Second import: Revoked.
        source.Transactions.Clear();
        await service.ImportAsync();

        Assert.Equal(
            "Revoked",
            (await database.Context.Transactions.SingleAsync())
                .Status);

        // Third import: Active again.
        source.Transactions.Add(CreateTransaction(
            transactionTime: transactionTime));

        await service.ImportAsync();

        var transaction = await database.Context.Transactions
            .SingleAsync();

        Assert.Equal("Active", transaction.Status);
        Assert.Null(transaction.RevokedAtUtc);

        var audits = await database.Context.TransactionAudits
            .ToListAsync();

        Assert.Equal(2, audits.Count);

        Assert.Contains(audits, audit =>
            audit.FieldName == "Status" &&
            audit.OldValue == "Active" &&
            audit.NewValue == "Revoked");

        Assert.Contains(audits, audit =>
            audit.FieldName == "Status" &&
            audit.OldValue == "Revoked" &&
            audit.NewValue == "Active");
    }

    private sealed class FakeSourceTransactionProvider
        : ISourceTransactionProvider
    {
        public string SourceSystem { get; }

        public List<SourceTransaction> Transactions { get; } = new();

        public FakeSourceTransactionProvider(string sourceSystem)
        {
            SourceSystem = sourceSystem;
        }

        public Task<IReadOnlyList<SourceTransaction>> GetTransactionsAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<SourceTransaction> snapshot =
                Transactions.ToList();

            return Task.FromResult(snapshot);
        }
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        public AppDbContext Context { get; }

        public TestDatabase(
            SqliteConnection connection,
            AppDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
