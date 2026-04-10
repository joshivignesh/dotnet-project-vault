# DbPerfDemo

This project is a simple .NET 10 app that uses Entity Framework Core.

## What was wrong

The app was using a SQLite file (`DbPerfDemo.db`). SQL Server Management Studio cannot open or run SQL Server commands against a SQLite database.

That is why these commands did not work:

```sql
SET STATISTICS IO ON;
SET STATISTICS TIME ON;
```

Those commands only work with SQL Server.

## What was fixed

- Changed the EF provider to SQL Server in `Program.cs`
- Updated `DbPerfDemo.csproj` to use `Microsoft.EntityFrameworkCore.SqlServer`
- Changed `appsettings.json` to use LocalDB:
  - `Server=(localdb)\mssqllocaldb;Database=DbPerfDemo;Trusted_Connection=True;`
- Added SQL Server decimal precision for `Price` fields
- Regenerated the EF migration and applied it to create the SQL Server database

## How to use it now

1. Open SSMS
2. Connect to `(localdb)\mssqllocaldb`
3. Open the `DbPerfDemo` database
4. Run:

```sql
SET STATISTICS IO ON;
SET STATISTICS TIME ON;
```

Then run your query and you will see the SQL Server performance output.

## Notes

- You do not need to keep rerunning the old EF SQLite setup.
- The project now uses SQL Server LocalDB, so SSMS can connect and run the expected performance commands.
