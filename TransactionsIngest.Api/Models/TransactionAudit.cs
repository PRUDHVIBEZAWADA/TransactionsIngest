
namespace TransactionsIngest.Api.Models;

public class TransactionAudit
{
    public long Id { get; set; }

    public long TransactionRecordId { get; set; }

    public TransactionRecord TransactionRecord { get; set; } = null!;

    public string FieldName { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
