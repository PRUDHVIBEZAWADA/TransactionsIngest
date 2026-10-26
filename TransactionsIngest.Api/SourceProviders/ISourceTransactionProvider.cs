using TransactionsIngest.Api.Models;

namespace TransactionsIngest.Api.SourceProviders;

public interface ISourceTransactionProvider
{
    string SourceSystem { get; }

    Task<IReadOnlyList<SourceTransaction>> GetTransactionsAsync(
        CancellationToken cancellationToken = default);
}
