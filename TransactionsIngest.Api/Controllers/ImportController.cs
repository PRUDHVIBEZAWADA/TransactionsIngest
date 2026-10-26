
using Microsoft.AspNetCore.Mvc;
using TransactionsIngest.Api.Services;

namespace TransactionsIngest.Api.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController : ControllerBase
{
    private readonly TransactionImportService _importService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(
        TransactionImportService importService,
        ILogger<ImportController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Import(
        CancellationToken cancellationToken)
    {
        try
        {
            await _importService.ImportAsync(cancellationToken);

            return Ok(new
            {
                Message = "Transaction import completed successfully."
            });
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Transaction import request was cancelled.");

            return StatusCode(
                499,
                new { Message = "Transaction import was cancelled." });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred during transaction import.");

            return StatusCode(
                500,
                new { Message = "Transaction import failed." });
        }
    }
}