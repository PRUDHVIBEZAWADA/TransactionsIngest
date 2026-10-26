
using TransactionsIngest.Api.Models;

namespace TransactionsIngest.Api.SourceProviders;

public class StoreUptownSource : ISourceTransactionProvider
{
    public string SourceSystem => "Store Uptown";

    public Task<IReadOnlyList<SourceTransaction>> GetTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SourceTransaction> transactions =
            new List<SourceTransaction>
            {
                new SourceTransaction
                {
                    TransactionId = 100001,
                    CardNumber = "***1111",
                    LocationCode = "UPTOWN",
                    ProductName = "Wireless Mouse",
                    Amount = 24.99m,
                    TransactionTime = new DateTime(
                        2026, 2, 27, 8, 15, 30, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100002,
                    CardNumber = "***2222",
                    LocationCode = "UPTOWN",
                    ProductName = "USB Cable",
                    Amount = 12.50m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 22, 0, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100003,
                    CardNumber = "***3333",
                    LocationCode = "UPTOWN",
                    ProductName = "Keyboard",
                    Amount = 49.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 19, 0, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100004,
                    CardNumber = "***4444",
                    LocationCode = "UPTOWN",
                    ProductName = "Monitor",
                    Amount = 189.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 24, 18, 0, 0, DateTimeKind.Utc)
                },
                new SourceTransaction
                {
                    TransactionId = 100005,
                    CardNumber = "***5555",
                    LocationCode = "UPTOWN",
                    ProductName = "Headphones",
                    Amount = 79.99m,
                    TransactionTime = new DateTime(
                        2026, 10, 25, 14, 0, 0, DateTimeKind.Utc)
                }
            };

        return Task.FromResult(transactions);
    }
}
