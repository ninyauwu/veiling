using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Veiling.Server;

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
}

// Part 2 - General variables
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Part 3 - Build connection string
var connectionString
    = builder.Configuration.GetConnectionString("Default")
    + $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};"
    + $"User Id={Environment.GetEnvironmentVariable("DB_USERNAME")};";
Console.WriteLine($"Connecting to database via {connectionString}");
if (password?.Length < 8) Console.WriteLine("Warning: Password should be at least 8 characters.");
connectionString += $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};"; 

// Add services to the container.
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(connectionString));

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    try
    {
        var canConnect = await db.Database.ExecuteSqlRawAsync("SELECT 1");
        app.Logger.LogInformation("Successfully connected to database and executed test query.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to connect to database or execute query.");
    }

    if (app.Environment.IsDevelopment())
    {
        AppDbSeeder.Seed(db);
    }
}

app.Run();


