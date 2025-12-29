using Serilog;
using Microsoft.EntityFrameworkCore;
using BookstoreApp.Api.Data;
using BookstoreApp.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("BookstoreDbConnection");
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapperConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
