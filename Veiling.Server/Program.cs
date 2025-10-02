using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Veiling.Server;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ==Create connection string==
// Part 1 - Container-specific variables
string? db_server;
string? db_username;
var containerized = builder.Configuration["Docker:IsContainerized"] != "False";
if (!containerized) {
    Env.TraversePath().Load();
    db_server = Environment.GetEnvironmentVariable("DB_SERVER");
    db_username = Environment.GetEnvironmentVariable("DB_USERNAME");
} else {
    Console.WriteLine("Running via docker compose. Setting up database link.");
    db_server = "tcp:db,1433";
    db_username = "sa";
}

// Part 2 - General variables
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Part 3 - Build connection string
var connectionString
    = builder.Configuration.GetConnectionString("Default")
    + $"Server={db_server};"
    + $"User Id={db_username};";
Console.WriteLine($"Connecting to database via {connectionString} "
        + $"with a password of {password?.Length} chars long.");
if (password?.Length < 8) Console.WriteLine("Warning: Password should be at least 8 characters.");
connectionString += $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";
ConnectionStore.ConnectionString = connectionString;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    using (var command = new SqlCommand("SELECT 1", connection)) // 👈 pass connection
    {
        var result = command.ExecuteScalar();
        Console.WriteLine($"First User: {result}");
    }
}

app.Run();

