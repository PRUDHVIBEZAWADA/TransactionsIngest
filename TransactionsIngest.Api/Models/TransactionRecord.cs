
namespace TransactionsIngest.Api.Models;

public class TransactionRecord
{
    public long Id { get; set; }

    public string SourceSystem { get; set; } = string.Empty;

    public int TransactionId { get; set; }

    public string CardNumber { get; set; } = string.Empty;

    public string LocationCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionTime { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime FirstSeenAtUtc { get; set; }

    public DateTime LastSeenAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
}
