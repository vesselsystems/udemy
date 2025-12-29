# Scaffold Database (SQL Server, Database-First)

Quick notes for setting up a database-first schema you can reuse on future projects.

## Create DB

```sh
docker exec mssql-2022 /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrongP@ss!' -C \
  -Q "CREATE DATABASE BookstoreDb; SELECT name FROM sys.databases WHERE name='BookstoreDb';"
```

## Create Tables (Authors + Books)

```sh
docker exec mssql-2022 /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrongP@ss!' -C -d BookstoreDb \
  -Q "CREATE TABLE dbo.Authors (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, FirstName NVARCHAR(50) NOT NULL, LastName NVARCHAR(50) NOT NULL, Bio NVARCHAR(250) NULL); CREATE TABLE dbo.Books (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, Title NVARCHAR(50) NOT NULL, Year INT NOT NULL, Isbn NVARCHAR(50) NOT NULL UNIQUE, Summary NVARCHAR(250) NULL, Image NVARCHAR(50) NULL, Price DECIMAL(18,2) NOT NULL, AuthorId INT NULL, CONSTRAINT FK_Books_Authors FOREIGN KEY (AuthorId) REFERENCES dbo.Authors(Id));"
```

## Schema Notes

- Authors: `Id` identity PK, `FirstName`, `LastName`, `Bio`.
- Books: `Id` identity PK, `Title`, `Year`, `Isbn` (unique), `Summary`, `Image`, `Price`, `AuthorId` (FK).
- Foreign key: `Books.AuthorId` → `Authors.Id`.
- `DECIMAL(18,2)` supports 18 digits with 2 decimal places.

## EF Core Scaffolding (database-first)

Install packages:

```sh
dotnet add BookstoreApp.Api package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.8
dotnet add BookstoreApp.Api package Microsoft.EntityFrameworkCore.Tools --version 9.0.8
dotnet add BookstoreApp.Api package Microsoft.EntityFrameworkCore.Design --version 9.0.8
```

Add connection string in `BookstoreApp.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "BookstoreDbConnection": "Server=localhost,1433;Database=BookstoreDb;User Id=sa;Password=Boomer732!;Encrypt=False;MultipleActiveResultSets=True"
  }
}
```

Wire up EF Core in `BookstoreApp.Api/Program.cs`:

```csharp
using BookstoreApp.Api.Data;
using Microsoft.EntityFrameworkCore;

var connectionString = builder.Configuration.GetConnectionString("BookstoreDbConnection");
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Install the EF CLI (one time):

```sh
dotnet tool install --global dotnet-ef --version 9.0.8
```

Scaffold the database:

```sh
dotnet-ef dbcontext scaffold "Server=localhost,1433;Database=BookstoreDb;User Id=sa;Password=Boomer732!;Encrypt=False;MultipleActiveResultSets=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project BookstoreApp.Api --startup-project BookstoreApp.Api \
  --output-dir Data --context BookstoreDbContext --context-dir Data \
  --no-onconfiguring --force
```

Notes:

- `Encrypt=False` avoids the SSL certificate error during scaffolding.
- `--no-onconfiguring` keeps the connection string out of the generated `DbContext`.
