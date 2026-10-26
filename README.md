# Transactions Ingest

A full-stack transaction ingestion application developed using **ASP.NET Core Web API (.NET 10), Angular 22, Entity Framework Core Code First, and SQLite**.

The application imports transactions from three simulated source systems, maintains transaction records and audit history, applies transaction lifecycle rules, and displays the imported data through an Angular dashboard.

## Technologies Used

| Component | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 10), C# |
| Frontend | Angular 22, TypeScript, HTML, CSS |
| Database | SQLite |
| ORM | Entity Framework Core Code First |
| Automated Testing | xUnit |
| Backend IDE | Visual Studio 2026 Community |
| Frontend IDE | Visual Studio Code |
| Database Inspection | DB Browser for SQLite |
| Version Control | Git and GitHub |

## Project Structure

```text
TransactionsIngest/
├── TransactionsIngest.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── SourceProviders/
│   ├── Properties/
│   ├── appsettings.json
│   └── TransactionsIngest.Api.csproj
│
├── TransactionsIngest.Tests/
│   └── TransactionsIngest.Tests.csproj
│
├── transactions-ingest-ui/
│   ├── src/
│   ├── public/
│   ├── angular.json
│   ├── package.json
│   ├── package-lock.json
│   └── proxy.conf.json
│
├── .gitignore
├── README.md
└── AI_USAGE.md
```

The backend is organized into separate folders for controllers, models, database access, source providers, and transaction processing. The Angular frontend and xUnit test project are maintained separately.

## Application Features

### Transaction Import

The application imports transactions from three simulated sources:

- Store Uptown
- Store Downtown
- Online

Each source contains five sample transactions, providing **15 transactions** for a fresh import.

Each transaction is identified using the combination of `SourceSystem` and `TransactionId`. This allows different sources to use the same transaction ID without being treated as duplicates.

### Transaction Lifecycle

The application supports three transaction statuses:

| Status | Description |
|---|---|
| Active | A transaction that is currently present in its source and remains within the permitted lifecycle period |
| Revoked | A transaction removed from its source within the permitted 24-hour period |
| Finalized | A transaction whose 24-hour period has expired; its stored record is preserved |

The import process applies the following rules:

- New transactions are inserted into the database.
- Existing transactions are checked for changes.
- Relevant changes are recorded in the audit history.
- Transactions missing from their source can be revoked within the permitted 24-hour period.
- Revoked transactions can be reactivated if they reappear within that period.
- Transactions are finalized after the 24-hour period.
- Finalized transactions remain unchanged even if their source data is modified or removed.
- Repeated imports of unchanged data do not create duplicate transactions or unnecessary audit records.

### Transaction Audit History

The application maintains audit records to track changes to imported transactions.

Audit history is stored separately from the main transaction records, allowing changes to be reviewed without replacing the current transaction data.

### Angular Dashboard

The Angular dashboard provides:

- Total transaction count
- Active transaction count
- Revoked transaction count
- Finalized transaction count
- A table of imported transactions
- **Import Transactions** button
- **Refresh** button

The dashboard communicates with the ASP.NET Core backend through API requests.

## Prerequisites

Install the following tools before running the application:

1. **Visual Studio 2026 Community** with ASP.NET Core development support.
2. **.NET 10 SDK**.
3. **Node.js and npm**.
4. **Angular CLI** compatible with Angular 22.
5. **Visual Studio Code** for the Angular frontend.

Optional:

- **DB Browser for SQLite** to inspect and manage the local database.

## Getting Started

### Step 1 — Clone or Download the Repository

Clone the repository using Git or select **Code → Download ZIP** on GitHub.

Extract the downloaded files if using ZIP.

The project contains the backend API, Angular frontend, automated tests, and existing Entity Framework Core migrations.

### Step 2 — Open the Backend in Visual Studio

Open the backend project in Visual Studio 2026 Community:

`TransactionsIngest.Api/TransactionsIngest.Api.csproj`

Restore NuGet packages and build the project.

If working with a solution that includes the test project, ensure both the API and test projects are loaded.

### Step 3 — Create the SQLite Database Using EF Core Migrations

**Important: The `transactions.db` file is intentionally not included in this GitHub repository.**

This project uses **Entity Framework Core Code First migrations** to create and manage the database schema.

The existing migration files are included in:

`TransactionsIngest.Api/Migrations/`

These migration files allow a reviewer to create the SQLite database locally after downloading the repository.

#### Open Package Manager Console

In Visual Studio 2026 Community:

1. Select **Tools → NuGet Package Manager → Package Manager Console**.
2. Set the **Default project** to `TransactionsIngest.Api`.
3. Ensure the API project is available as the startup project.

#### Entity Framework Core Migration Commands

**Create an initial migration:**

```powershell
Add-Migration InitialCreate
```

This command generates migration files based on the entity models and `AppDbContext`.

**Apply migrations and create the database:**

```powershell
Update-Database
```

This command applies pending migrations and creates the SQLite database and required tables if they do not already exist.

#### Which Command Should a Reviewer Run?

The `InitialCreate` migration has **already been created during development** and is included in this repository.

Therefore, when downloading this project for the first time, **do not run `Add-Migration InitialCreate` again**.

Run only:

```powershell
Update-Database
```

This applies the existing migration and creates the local SQLite database.

The resulting database contains tables including:

- `Transactions`
- `TransactionAudits`
- `__EFMigrationsHistory`

The `__EFMigrationsHistory` table tracks which migrations have been applied.

#### SQLite Connection String

The database connection string is configured in `TransactionsIngest.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=transactions.db"
  }
}
```

The database filename is relative to the working directory used by the application or EF Core tooling.

When running the backend from its project directory, the expected database location is:

`TransactionsIngest.Api/transactions.db`

Use a consistent working directory for migration commands and API execution so they reference the same database file.

#### Alternative: Using the .NET CLI

From the `TransactionsIngest.Api` directory, install the EF Core CLI tool if needed:

```bash
dotnet tool install --global dotnet-ef
```

Then apply the existing migration:

```bash
dotnet ef database update
```

If `dotnet-ef` is already installed, there is no need to install it again.

#### Future Database Model Changes

If the entity models or `AppDbContext` change in the future, create a new migration with a descriptive name and apply it.

For example:

```powershell
Add-Migration AddTransactionField
Update-Database
```

These commands are for future schema changes.

For the initial setup of this repository, the existing migration only needs to be applied using `Update-Database`.

### Step 4 — Run the Backend API

In Visual Studio 2026 Community:

1. Set `TransactionsIngest.Api` as the startup project.
2. Select the **HTTPS** launch profile.
3. Press **F5** to run the application.

The backend runs at:

`https://localhost:7280`

To view the stored transactions, open:

`https://localhost:7280/api/transactions`

On a newly created database, the endpoint should initially return:

```json
[]
```

This is expected because the database schema has been created but no transactions have been imported yet.

### Step 5 — Run the Angular Frontend

Open the `transactions-ingest-ui` folder in Visual Studio Code.

Open a terminal inside the Angular project directory.

Install the required dependencies:

```bash
npm install
```

Start the Angular development server:

```bash
ng serve --port 4300 --proxy-config proxy.conf.json --open
```

The frontend opens at:

`http://localhost:4300`

Keep the ASP.NET Core backend running while using the dashboard.

The Angular proxy configuration forwards API requests to the backend during local development.

### Step 6 — Import Transactions

Once the backend and Angular frontend are running:

1. Open the Angular dashboard.
2. Click **Import Transactions**.
3. Wait for the import operation to complete.
4. Click **Refresh** if needed.

The application imports sample transactions from the three source providers.

| Source | Number of Transactions |
|---|---:|
| Store Uptown | 5 |
| Store Downtown | 5 |
| Online | 5 |
| **Total** | **15** |

The Active, Revoked, and Finalized counts depend on the UTC timestamps and the current time.

Some sample transactions have older timestamps to demonstrate finalization. Because the timestamps are fixed, their statuses may change as time passes.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/transactions` | Retrieves stored transactions |
| POST | `/api/import` | Imports and reconciles transactions from all source providers |

The Angular frontend uses these endpoints to retrieve transaction data and trigger imports.

## Automated Testing

The project includes an xUnit test project named `TransactionsIngest.Tests`.

The test suite covers eight scenarios:

1. Inserting a new transaction.
2. Preventing duplicate transactions during repeated imports.
3. Handling the same transaction ID from different source systems.
4. Updating a transaction amount and recording an audit entry.
5. Revoking a transaction that is missing from its source.
6. Finalizing a transaction after the permitted 24-hour period.
7. Preserving a finalized transaction when its source data changes.
8. Reactivating a revoked transaction when it reappears within the permitted period.

### Running Tests in Visual Studio

1. Open the solution containing the backend and test projects.
2. Select **Test → Test Explorer**.
3. Click **Run All Tests**.

All eight tests passed during local development.

## Manual Testing

Manual testing was performed using the Angular dashboard, backend API responses, and DB Browser for SQLite.

The testing process included:

- Importing all 15 sample transactions.
- Repeating the import to check duplicate prevention.
- Comparing dashboard totals with the API and database records.
- Calculating expected transaction status counts based on UTC timestamps.
- Removing an Active transaction from Store Uptown to test revocation.
- Removing a Finalized transaction from Store Downtown to verify that the stored record remained unchanged.
- Inspecting database records after import operations.

The source provider files were restored to five transactions each after the manual lifecycle tests.

## Resetting the Database for Testing

To clear imported transaction data and perform a fresh import without recreating the database:

1. Stop the backend application.
2. Open the correct `transactions.db` file in DB Browser for SQLite.
3. Select the **Execute SQL** tab.
4. Run:

```sql
DELETE FROM TransactionAudits;
DELETE FROM Transactions;
```

5. Click **Write Changes**.
6. Confirm that both tables are empty.
7. Close DB Browser for SQLite.
8. Restart the backend and import the sample transactions again.

**Do not delete** the database schema or `__EFMigrationsHistory` table.

Clearing transaction rows does not require running `Add-Migration` or `Update-Database` again because the database structure remains intact.

## Design Decisions

### Separate Source Providers

Each simulated source implements a shared `ISourceTransactionProvider` interface.

This separates source-specific data retrieval from transaction processing and allows additional providers to be added without changing the overall import approach.

### Composite Transaction Identity

Transactions are identified by the combination of `SourceSystem` and `TransactionId`.

This avoids conflicts when different sources contain transactions with the same numeric ID.

### Centralized Import Processing

The `TransactionImportService` contains the transaction reconciliation and lifecycle processing logic.

This keeps business rules separate from the API controllers.

### UTC-Based Lifecycle Rules

UTC timestamps are used for transaction lifecycle calculations to avoid inconsistencies caused by local time zones.

### Finalized Transaction Protection

Once a transaction is finalized, subsequent imports do not modify or revoke its stored record.

### Idempotent Imports

Repeated imports of unchanged data do not create duplicate transactions or unnecessary audit history entries.

### Entity Framework Core Migrations

EF Core Code First migrations manage the database schema.

The migration files are committed to GitHub, while the generated SQLite database file is excluded. This allows a reviewer to recreate the database locally without requiring a pre-populated database file.

## Notes and Limitations

- The three source providers use simulated in-memory transaction data rather than external systems.
- The application is intended for local development and demonstration.
- Sample transaction timestamps are fixed, so status distributions change over time.
- The SQLite database file is excluded from GitHub and must be created locally using the existing EF Core migration.
- The application does not include production authentication, deployment configuration, or external transaction source integrations.

## AI Usage Disclosure

AI assistance used during development is documented in [`AI_USAGE.md`](AI_USAGE.md).

The disclosure includes representative prompts, development instructions, conventions, design discussions, corrections to generated output, and testing activities.