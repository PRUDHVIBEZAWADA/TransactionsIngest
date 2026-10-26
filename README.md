\# Transactions Ingest



A full-stack application built using \*\*ASP.NET Core Web API (.NET 10), Angular 22, Entity Framework Core, and SQLite\*\*.



The application imports transactions from three different sources into one database. It tracks transaction changes, manages transaction statuses, and keeps an audit history.



I built this project to demonstrate how transaction data from multiple systems can be processed and managed in one place.



\## Technologies and Tools Used



\### Backend

\- C# and ASP.NET Core Web API (.NET 10)

\- Entity Framework Core (Code First)

\- SQLite

\- xUnit for automated testing

\- \*\*Visual Studio 2026 Community\*\* for backend development, debugging, database migrations, and testing



\### Frontend

\- Angular 22

\- TypeScript, HTML, and CSS

\- \*\*Visual Studio Code (VS Code)\*\* for frontend development

\- Angular CLI and npm



\### Database

\- SQLite for storing transactions and audit history

\- DB Browser for SQLite for checking records, running SQL queries, and resetting test data



\## Project Structure



```text

TransactionsIngest/

│

├── TransactionsIngest.Api/

│   ├── Controllers/

│   ├── Data/

│   ├── Models/

│   ├── Services/

│   ├── SourceProviders/

│   │   ├── StoreUptownSource.cs

│   │   ├── StoreDowntownSource.cs

│   │   └── OnlineTransactionSource.cs

│   ├── Migrations/

│   ├── Program.cs

│   └── appsettings.json

│

├── TransactionsIngest.Tests/

│   └── UnitTest1.cs

│

├── transactions-ingest-ui/

│

└── README.md

```



\## How the Application Works



The application uses three sample transaction sources:



\- Store Uptown

\- Store Downtown

\- Online



Each source provides a list of transactions. When the user clicks \*\*Import Transactions\*\* on the Angular dashboard, the backend reads the latest transactions from all three sources and saves them in SQLite.



Transactions are identified using both \*\*Source System\*\* and \*\*Transaction ID\*\*.



For example, Store Uptown and Store Downtown can both have a transaction with ID `100001`. These are treated as separate transactions because they belong to different sources.



During an import, the application compares incoming transactions with the records already stored in the database.



It can insert new transactions, update existing ones, record changes, and manage transaction statuses.



\## Transaction Status Rules



The application supports three statuses.



\### Active



A transaction is Active while it is within the allowed 24-hour period and has not been revoked or finalized.



\### Revoked



If an Active transaction disappears from its source within the allowed 24-hour period, it becomes Revoked.



The transaction is not deleted from the database.



If the same transaction appears again within the allowed period, it can become Active again.



\### Finalized



Transactions are finalized after the 24-hour period according to the import rules.



Once a transaction becomes Finalized, it cannot be modified or revoked, even if its details change or it disappears from the source.



\## Audit History



The application records changes made to existing transactions.



For example, if the amount of an eligible transaction changes from `$24.99` to `$29.99`, the application updates the transaction and creates an audit record.



Audit records also track applicable status changes, such as:



\- Active to Revoked

\- Revoked to Active

\- Active to Finalized



This helps explain what happened to a transaction during the import process.



\## Database Setup



The project uses \*\*SQLite with Entity Framework Core Code First migrations\*\*.



The connection string is configured in `TransactionsIngest.Api/appsettings.json`:



```json

"ConnectionStrings": {

&#x20; "DefaultConnection": "Data Source=transactions.db"

}

```



I created the initial database schema using EF Core migrations in Visual Studio 2026 Community.



The migration files are included in the project.



\### Setting Up the Database for the First Time



If you are running the project on a new computer:



1\. Open the solution in Visual Studio 2026 Community.

2\. Set `TransactionsIngest.Api` as the startup project.

3\. Open \*\*Tools → NuGet Package Manager → Package Manager Console\*\*.

4\. Select `TransactionsIngest.Api` as the default project.

5\. Run:



```powershell

Update-Database

```



This applies the existing migrations and creates the required database tables.



\*\*You do not need to run `Add-Migration` again.\*\* The migration has already been created and is included in the project.



If you are using the existing initialized database, you do not need to apply migrations again.



\## Running the Backend



I used \*\*Visual Studio 2026 Community\*\* to develop and run the backend.



1\. Open the solution in Visual Studio.

2\. Set `TransactionsIngest.Api` as the startup project.

3\. Select the `https` launch profile.

4\. Press \*\*F5\*\*.



The backend runs at:



`https://localhost:7280`



The transactions endpoint is:



`https://localhost:7280/api/transactions`



The launch settings are configured to open the transactions endpoint automatically when the backend starts.



If the database is empty, the endpoint returns:



```json

\[]

```



\### API Endpoints



| Method | Endpoint | Description |

|---|---|---|

| GET | `/api/transactions` | Returns stored transactions |

| POST | `/api/import` | Imports transactions from all three sources |



\## Running the Angular Frontend



I used \*\*Visual Studio Code (VS Code)\*\* to develop the Angular frontend.



Make sure Node.js and npm are installed.



1\. Open the `transactions-ingest-ui` folder in VS Code.

2\. Open the integrated terminal.

3\. Install the project dependencies:



```bash

npm install

```



4\. Start Angular:



```bash

ng serve --port 4300 --proxy-config proxy.conf.json --open

```



The frontend opens at:



`http://localhost:4300`



Keep the backend running in Visual Studio while using the frontend.



The Angular development proxy forwards API requests to the backend.



\## Angular Dashboard



The frontend provides a simple dashboard for viewing transactions.



It displays:



\- Total Transactions

\- Active Transactions

\- Revoked Transactions

\- Finalized Transactions

\- A table of transaction details

\- A \*\*Refresh\*\* button

\- An \*\*Import Transactions\*\* button



The \*\*Import Transactions\*\* button runs the backend import process and refreshes the dashboard.



The \*\*Refresh\*\* button reloads the transactions currently stored in the database.



\## Sample Transaction Data



The project contains three source provider classes:



\- `StoreUptownSource.cs`

\- `StoreDowntownSource.cs`

\- `OnlineTransactionSource.cs`



Each source originally contained a sample transaction from the exercise.



I expanded the sample data to five transactions per source, giving \*\*15 transactions\*\* for testing.



The sample transactions include different products, amounts, card numbers, and transaction timestamps.



Some transaction IDs are intentionally repeated across different sources to test that the application correctly uses both the source name and transaction ID.



The source providers contain hardcoded sample data to simulate external transaction systems. They do not connect to real payment services.



\*\*Note:\*\* During manual testing, I removed some sample transactions from the source provider classes to test Revoked and Finalized behavior. To reproduce the original 15-transaction import from an empty database, restore all five sample transactions in each source provider before importing.



\## Starting Fresh with an Empty Database



During development, I used DB Browser for SQLite to clear existing transaction data and test the import process again.



This can be done without deleting the database or recreating its tables.



\### Steps to Clear Transaction Data



1\. Stop the backend in Visual Studio.

2\. Open \*\*DB Browser for SQLite\*\*.

3\. Open `TransactionsIngest.Api/transactions.db`.

4\. Select the \*\*Execute SQL\*\* tab.

5\. Run:



```sql

DELETE FROM TransactionAudits;

DELETE FROM Transactions;

```



6\. Click \*\*Write Changes\*\* to save the changes.

7\. Go to \*\*Browse Data\*\*.

8\. Confirm that both `Transactions` and `TransactionAudits` are empty.

9\. Close DB Browser for SQLite.

10\. Restart the backend in Visual Studio.

11\. Start Angular in VS Code.

12\. Click \*\*Import Transactions\*\*.



The application imports the transactions currently defined in the three source provider classes.



\### Important Database Notes



\- Do not delete the `\_\_EFMigrationsHistory` table.

\- Do not delete the migration files.

\- There is no need to run `Add-Migration` or `Update-Database` when simply clearing transaction records.

\- Make sure DB Browser opens the same `transactions.db` file used by the backend.

\- The SQLite connection uses a relative path, so database files can exist in different folders.

\- This reset process is intended for local testing only.



\## How to Test the Application



The sample source provider classes make it possible to test the transaction rules manually.



\### Test 1 — Import New Transactions



Start with an empty database and import the sample transactions.



If all three source providers contain five records, the dashboard should display \*\*15 total transactions\*\*.



The Active and Finalized counts depend on the timestamps and when the import is performed.



\### Test 2 — Revoke an Active Transaction



1\. Import an Active transaction that is still within the allowed 24-hour period.

2\. Stop the backend.

3\. Open the source provider containing that transaction.

4\. Remove the transaction from its source list.

5\. Save the file.

6\. Restart the backend.

7\. Click \*\*Import Transactions\*\* again.



The transaction should remain in SQLite but change to Revoked.



I tested this using the \*\*USB Cable transaction (100002)\*\* from Store Uptown.



\### Test 3 — Remove a Finalized Transaction



1\. Find a transaction that is already Finalized.

2\. Stop the backend.

3\. Remove that transaction from its source provider.

4\. Restart the backend.

5\. Import again.



The transaction should remain Finalized in the database.



I tested this using the \*\*Charger transaction (100004)\*\* from Store Downtown.



This confirms that finalized transactions cannot be changed or revoked.



\### Test 4 — Update an Existing Transaction



Change the amount or another tracked field of an eligible transaction in its source provider.



Import again.



The stored transaction should be updated, and the change should appear in the audit history.



\### Test 5 — Import the Same Data Twice



Import the same unchanged source data multiple times.



The application should not create duplicate transactions or unnecessary audit records.



This verifies that the import process is idempotent.



\### Test 6 — Reactivate a Revoked Transaction



1\. Remove an eligible Active transaction from its source.

2\. Import again so it becomes Revoked.

3\. Add the same transaction back to the source.

4\. Import again.



If the transaction is still within the allowed 24-hour period, it should become Active again.



\## Automated Testing



The backend includes an xUnit test project named `TransactionsIngest.Tests`.



I wrote eight tests to verify the main import rules:



1\. Importing a new transaction

2\. Preventing duplicates when importing unchanged data

3\. Handling the same transaction ID from different sources

4\. Updating a transaction amount and creating an audit

5\. Revoking a missing transaction

6\. Finalizing an older transaction

7\. Preventing changes to Finalized transactions

8\. Reactivating a Revoked transaction



The tests use an in-memory SQLite database, so they do not affect the normal application database.



\### Running the Tests



In Visual Studio 2026 Community:



1\. Open \*\*Test → Test Explorer\*\*.

2\. Click \*\*Run All Tests\*\*.

3\. Check the results.



During local testing, \*\*all 8 tests passed\*\*.



\## Design Decisions



I made a few design choices to keep the project simple and easy to maintain.



\*\*Separate source providers\*\*



Each transaction source has its own class implementing the same interface. This makes it easier to add more sources without changing the main import logic.



\*\*Central import service\*\*



The import service handles transaction comparison, updates, status changes, and audit history in one place.



\*\*SQLite database\*\*



SQLite is lightweight and does not require a separate database server, which makes local setup easier.



\*\*EF Core Code First\*\*



I used Code First migrations to create and manage the database schema from the C# models.



\*\*Angular frontend\*\*



I used Angular to create a simple dashboard where transactions can be viewed and imported.



\*\*Automated tests\*\*



I used xUnit with in-memory SQLite to verify the business rules independently of the application's main database.



\## Notes and Limitations



\- Transaction sources are simulated using hardcoded data.

\- Importing is triggered manually rather than on a schedule.

\- Fixed sample timestamps become older over time, which affects transaction statuses.

\- The application is designed for local development and demonstration.

\- A production application would need additional features such as authentication, real external integrations, monitoring, and deployment configuration.



\## Summary



This project demonstrates how transactions from multiple sources can be imported into one database while tracking changes and following transaction lifecycle rules.



The main focus was building a repeatable import process, managing Active, Revoked, and Finalized transactions, keeping an audit history, and providing a simple Angular dashboard to view and test the results.



I developed the backend using \*\*Visual Studio 2026 Community\*\*, built the frontend using \*\*Visual Studio Code\*\*, and used \*\*DB Browser for SQLite\*\* to inspect and test the database.



