
using TransactionsIngest.Api.Models;

namespace TransactionsIngest.Api.SourceProviders;

public class StoreDowntownSource : ISourceTransactionProvider
{
    public string SourceSystem => "Store Downtown";

    public Task<IReadOnlyList<SourceTransaction>> GetTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SourceTransaction> transactions =
            new List<SourceTransaction>
            {
                new SourceTransaction
                {
                    TransactionId = 100001,
                    CardNumber = "***0002",
                    LocationCode = "DOWNTOWN",
                    ProductName = "Mechanical Keyboard",
                    Amount = 89.99m,
                    TransactionTime = new DateTime(
                        2026, 2, 27, 9, 45, 12, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100002,
                    CardNumber = "***2222",
                    LocationCode = "DOWNTOWN",
                    ProductName = "Laptop Stand",
                    Amount = 39.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 20, 0, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100003,
                    CardNumber = "***3333",
                    LocationCode = "DOWNTOWN",
                    ProductName = "Webcam",
                    Amount = 64.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 18, 30, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100004,
                    CardNumber = "***4444",
                    LocationCode = "DOWNTOWN",
                    ProductName = "Charger",
                    Amount = 29.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 24, 12, 0, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100005,
                    CardNumber = "***5555",
                    LocationCode = "DOWNTOWN",
                    ProductName = "Speaker",
                    Amount = 119.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 22, 0, 0, DateTimeKind.Utc)
                }
            };

        return Task.FromResult(transactions);
    }
}
