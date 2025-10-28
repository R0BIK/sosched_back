using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Storage;

var builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

var connectionString = builder.Configuration.GetConnectionString("Local")
                       ?? throw new InvalidOperationException("Connection string 'Local' not found.");

connectionString = connectionString
    .Replace("${POSTGRES_HOST}", Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost")
    .Replace("${POSTGRES_DB}",
        Environment.GetEnvironmentVariable("POSTGRES_DB") ??
        throw new InvalidOperationException("POSTGRES_DB environment variable not set"))
    .Replace("${POSTGRES_USER}",
        Environment.GetEnvironmentVariable("POSTGRES_USER") ??
        throw new InvalidOperationException("POSTGRES_USER environment variable not set"))
    .Replace("${POSTGRES_PASSWORD}",
        Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
        throw new InvalidOperationException("POSTGRES_PASSWORD environment variable not set"));

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'Local' not found or environment variables missing.");

builder.Services.AddDbContext<SoschedBackDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
