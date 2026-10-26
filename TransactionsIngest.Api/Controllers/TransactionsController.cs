
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionsIngest.Api.Data;

namespace TransactionsIngest.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        AppDbContext dbContext,
        ILogger<TransactionsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _dbContext.Transactions
                .AsNoTracking()
                .OrderBy(t => t.SourceSystem)
                .ThenBy(t => t.TransactionId)
                .Select(t => new
                {
                    t.SourceSystem,
                    t.TransactionId,
                    t.CardNumber,
                    t.LocationCode,
                    t.ProductName,
                    t.Amount,
                    t.TransactionTime,
                    t.Status
                })
                .ToListAsync(cancellationToken);

            return Ok(transactions);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Transaction retrieval request was cancelled.");

            return StatusCode(
                499,
                new { Message = "Transaction retrieval was cancelled." });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while retrieving transactions.");

            return StatusCode(
                500,
                new { Message = "Failed to retrieve transactions." });
        }
    }
}
