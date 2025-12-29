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
