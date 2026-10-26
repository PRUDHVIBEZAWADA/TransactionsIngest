
using TransactionsIngest.Api.Models;

namespace TransactionsIngest.Api.SourceProviders;

public class OnlineTransactionSource : ISourceTransactionProvider
{
    public string SourceSystem => "Online";

    public Task<IReadOnlyList<SourceTransaction>> GetTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<SourceTransaction> transactions =
        [
            // Original transaction from the exercise.
            new SourceTransaction
            {
                TransactionId = 100001,
                CardNumber = "***4444",
                LocationCode = "ONLINE",
                ProductName = "USB-C Docking Station",
                Amount = 129.95m,
                TransactionTime = new DateTime(
                    2026, 2, 27, 11, 20, 5,
                    DateTimeKind.Utc)
            },

            new SourceTransaction
            {
                TransactionId = 100002,
                CardNumber = "***2222",
                LocationCode = "ONLINE",
                ProductName = "Gaming Mouse",
                Amount = 59.99m,
                TransactionTime = new DateTime(
                    2026, 10, 25, 21, 0, 0,
                    DateTimeKind.Utc)
            },

            new SourceTransaction
            {
                TransactionId = 100003,
                CardNumber = "***3333",
                LocationCode = "ONLINE",
                ProductName = "USB Hub",
                Amount = 34.50m,
                TransactionTime = new DateTime(
                    2026, 10, 25, 19, 0, 0,
                    DateTimeKind.Utc)
            },

            new SourceTransaction
            {
                TransactionId = 100004,
                CardNumber = "***4445",
                LocationCode = "ONLINE",
                ProductName = "External SSD",
                Amount = 149.99m,
                TransactionTime = new DateTime(
                    2026, 10, 24, 10, 0, 0,
                    DateTimeKind.Utc)
            },

            new SourceTransaction
            {
                TransactionId = 100005,
                CardNumber = "***5555",
                LocationCode = "ONLINE",
                ProductName = "Laptop Sleeve",
                Amount = 22.99m,
                TransactionTime = new DateTime(
                    2026, 10, 25, 23, 0, 0,
                    DateTimeKind.Utc)
            }
        ];

        return Task.FromResult(transactions);
    }
}
