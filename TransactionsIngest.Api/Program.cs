
using Microsoft.EntityFrameworkCore;
using TransactionsIngest.Api.Data;
using TransactionsIngest.Api.Services;
using TransactionsIngest.Api.SourceProviders;

var builder = WebApplication.CreateBuilder(args);

// Register controllers and OpenAPI.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register the three transaction source providers.
builder.Services.AddScoped<ISourceTransactionProvider, StoreUptownSource>();
builder.Services.AddScoped<ISourceTransactionProvider, StoreDowntownSource>();
builder.Services.AddScoped<ISourceTransactionProvider, OnlineTransactionSource>();

// Register the transaction import service.
builder.Services.AddScoped<TransactionImportService>();

// Read the SQLite connection string from appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

// Register the EF Core database context.
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(connectionString));

var app = builder.Build();

// Enable OpenAPI in development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
