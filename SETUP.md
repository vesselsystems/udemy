# Setup Quickstart (macOS + .NET + SQL Server Docker)

Use this as a repeatable checklist for new API projects.

## 1) Create solution + API

```sh
mkdir MyProject
cd MyProject
dotnet new sln -n MyProject
dotnet new webapi -n MyProject.Api --framework net9.0 --use-controllers
dotnet sln add MyProject.Api/MyProject.Api.csproj
```

## 2) Add Swagger UI (if needed)

```sh
dotnet add MyProject.Api package Swashbuckle.AspNetCore --version 7.0.0
```

Update `MyProject.Api/Program.cs`:

```csharp
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

## 3) Run

```sh
dotnet run --project MyProject.Api
```

Open:
```
http://localhost:<port>/swagger
```

## 4) Serilog + Seq (optional)

Install packages:

```sh
dotnet add MyProject.Api package Serilog.AspNetCore
dotnet add MyProject.Api package Serilog.Expressions
dotnet add MyProject.Api package Serilog.Settings.Configuration
dotnet add MyProject.Api package Serilog.Sinks.File
dotnet add MyProject.Api package Serilog.Sinks.Seq
```

Update `MyProject.Api/Program.cs`:

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));
```

Add to `MyProject.Api/appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ]
  }
}
```

Run Seq (Docker):

```sh
docker rm -f seq
docker run --name seq -d \
  -e ACCEPT_EULA=Y \
  -e SEQ_FIRSTRUN_ADMINPASSWORD=YourStrongPassword123! \
  -p 5341:80 \
  -p 5342:5341 \
  datalust/seq:latest
```

Open:
```
http://localhost:5341
```

## 5) CORS (allow all for local dev)

Update `MyProject.Api/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});
```

Then in the middleware pipeline:

```csharp
app.UseHttpsRedirection();
app.UseCors("AllowAll");
```

## 6) SQL Server (Docker)

```sh
docker volume create mssql_data
docker run -d --name mssql-2022 \
  -e "ACCEPT_EULA=Y" \
  -e 'MSSQL_SA_PASSWORD=YourStrongP@ss!' \
  -p 1433:1433 \
  -v mssql_data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```

## 7) Create DB

```sh
docker exec mssql-2022 /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrongP@ss!' -C \
  -Q "CREATE DATABASE MyDb; SELECT name FROM sys.databases WHERE name='MyDb';"
```

## 8) appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MyDb;User Id=sa;Password=YourStrongP@ss!;Encrypt=True;TrustServerCertificate=True"
  }
}
```

## 9) Git (optional)

```sh
git init
dotnet new gitignore
git add .
git commit -m "Initial commit"
```

## 10) GitHub (optional)

Create a repo and push:

```sh
git branch -M main
git remote add origin https://github.com/<your-username>/<your-repo>.git
git push -u origin main
```
