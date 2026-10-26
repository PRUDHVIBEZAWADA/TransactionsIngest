# AI Usage Disclosure

## Overview

I used ChatGPT during this assignment to improve development efficiency, particularly for repetitive code generation, sample test data, specific business-rule refinements, CSS styling, automated testing, troubleshooting, and documentation.

Before starting development, I had an idea of how I wanted to structure the application. I organized the project into the required components and worked through the implementation based on the assignment requirements.

I used AI assistance for development tasks where it could save time, provided existing code and requirements where applicable, and checked the generated output through manual and automated testing.

## Development Approach

I started by reviewing the assignment requirements and identifying the main responsibilities of the application:

- Import transactions from Store Uptown, Store Downtown, and Online.
- Store transaction details and maintain audit history.
- Handle Active, Revoked, and Finalized transaction statuses.
- Apply the required 24-hour lifecycle rules.
- Avoid duplicate transactions and unnecessary audit entries.
- Expose transaction data through API endpoints.
- Display transaction records and status counts in an Angular dashboard.

I organized the backend into models, controllers, services, source providers, and data access.

The technologies used were:

- **ASP.NET Core Web API (.NET 10)** — Visual Studio 2026 Community
- **Angular 22** — Visual Studio Code
- **Entity Framework Core Code First with SQLite**
- **xUnit** for automated testing

I also shared the assignment PDF with ChatGPT to help organize the requirements and discuss implementation and testing scenarios.

## AI-Assisted Code Generation

### Transaction Audit Model

I had created the `Transaction` model using the fields required by the assignment.

I provided the existing model to ChatGPT and asked it to help create a corresponding `TransactionAudit` model, following the same naming conventions and supporting the tracking of transaction changes.

This helped reduce repetitive coding while keeping the related models consistent.

### Sample Data for Source Providers

I initially created `StoreUptownSource.cs` with one sample transaction.

I shared the existing file with ChatGPT and asked it to add four more transactions using the same structure.

I followed a similar approach for Store Downtown and Online, preparing five sample transactions for each source.

This gave me 15 sample transactions for testing imports, transaction statuses, and dashboard calculations.

### Finalized Status Logic

I used ChatGPT to assist with refining the Finalized status behavior in the transaction import process.

I worked through the requirement that transactions become finalized after the permitted 24-hour period and that finalized records must remain unchanged even if their source data is subsequently modified or removed.

I verified this behavior using automated tests and manual testing with the Downtown source.

## Corrections to Generated Test Data

Some initially generated sample timestamps did not produce the expected results for the transaction lifecycle scenarios.

I corrected the UTC timestamps so that the test data contained appropriate Active and Finalized transactions.

After importing the records, I compared the actual statuses with the expected results based on the 24-hour rules.

I also checked that the dashboard counts matched the API responses and SQLite records.

This helped me identify incorrect test data rather than assuming that the generated values were suitable.

## Angular Dashboard CSS Styling

I used ChatGPT to help with the CSS styling of my existing Angular dashboard.

I provided the dashboard structure and explained the visual appearance I wanted.

AI helped generate and refine CSS for the transaction status cards, transaction table, spacing, colors, and action buttons.

I integrated the CSS into the Angular project and checked its appearance in the browser.

## Automated Testing

I used ChatGPT to help generate and refine xUnit tests based on the transaction rules and existing backend classes.

The final test suite covers eight scenarios:

1. Inserting a new transaction.
2. Preventing duplicate transactions.
3. Handling the same transaction ID across different sources.
4. Updating an amount and recording an audit entry.
5. Revoking a missing transaction.
6. Finalizing an older transaction.
7. Preserving a finalized transaction.
8. Reactivating a revoked transaction.

I adapted the generated tests to the existing implementation and worked through an issue with the SQLite test database setup.

I ran the tests in Visual Studio 2026 Community and confirmed that all eight passed during local development.

## Manual Testing and Data Validation

I used the 15 sample transactions to verify the application through the API, SQLite database, and Angular dashboard.

I manually calculated the expected Total, Active, Revoked, and Finalized counts and compared them with the dashboard results.

I also checked whether the individual status counts added up to the total number of transactions.

To test specific lifecycle scenarios, I removed an Active transaction from Store Uptown and imported again to check revocation.

I also removed a Finalized transaction from Store Downtown and verified that its stored record remained unchanged.

I used DB Browser for SQLite to inspect transaction records and reset data when testing fresh imports.

These checks helped me validate the application's behavior against the expected results rather than relying only on successful builds or API responses.

## Troubleshooting

I used ChatGPT for assistance when investigating technical issues, including:

- Backend launch URL configuration.
- SQLite database file locations.
- Entity Framework Core setup.
- SQLite database cleanup during automated testing.
- Sample timestamps and transaction status calculations.

I reviewed the suggested corrections, applied the relevant changes, and verified the results afterward.

## Documentation

I used ChatGPT to help generate and organize `README.md`, including the project overview, setup instructions, technologies, transaction rules, and testing steps.

I also used ChatGPT to draft and refine this AI disclosure.

Representative prompts, instructions, conventions, and design discussions are documented separately in `AI_PROMPTS.md`.

## Representative Prompts

The following examples summarize the types of instructions I provided during development. They are reconstructed examples rather than exact transcripts.

**Audit model**

"I have already created the Transaction model. Based on its fields and structure, help me create a corresponding TransactionAudit model to track transaction changes."

**Sample source data**

"I have created StoreUptownSource.cs with one transaction. Add four more using the same structure. Similarly, prepare five sample transactions for Downtown and Online."

**Finalized status**

"Help me refine the Finalized status behavior so transactions older than 24 hours are finalized and cannot be changed or revoked afterward."

**Timestamp corrections**

"Some generated timestamps are producing incorrect transaction statuses. Help me adjust the UTC timestamps to test the Active and Finalized scenarios."

**Dashboard CSS**

"I have already created the Angular dashboard. Help me improve the CSS for the transaction cards, table, and buttons without changing the functionality."

**Automated tests**

"Using my existing backend classes and transaction rules, help me create xUnit tests for insertion, duplicate prevention, auditing, revocation, finalization, and reactivation."

**Documentation**

"Help me prepare the README and AI disclosure based on the application structure, implementation decisions, and testing performed."

## Development Conventions

During development, I followed these conventions:

- Maintain the existing project structure and naming conventions.
- Keep transaction processing separate from API controllers.
- Use the source system and transaction ID together to identify transactions.
- Use UTC timestamps for lifecycle calculations.
- Preserve finalized records.
- Avoid duplicate transactions and unnecessary audit entries.
- Use sample data to test different transaction scenarios.
- Compare actual results with expected manual counts.
- Verify changes through API responses, database inspection, and automated tests.

## Final Note

I used AI to make development more efficient, especially for repetitive code, sample data generation, Finalized status refinements, CSS styling, automated tests, troubleshooting, and documentation.

I provided existing code and requirements where applicable, reviewed and integrated generated output, corrected unsuitable timestamps, and validated the application through manual calculations, API responses, SQLite inspection, and automated tests.

I take responsibility for the final implementation and the verification performed before submission.