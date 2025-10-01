using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Veiling.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Create connection string
string? db_server;
string? db_username;
if (builder.Configuration["Docker:IsContainerized"] == "False") {
    Env.TraversePath().Load();
    db_server = Environment.GetEnvironmentVariable("DB_SERVER");
    db_username = Environment.GetEnvironmentVariable("DB_USERNAME");
} else {
    Console.WriteLine("Running via docker compose. Setting up database link.");
    db_server = "tcp:db,1433";
    db_username = "sa";
}
var connectionString
    = $"Server={db_server};"
    + $"Database={Environment.GetEnvironmentVariable("DB_NAME")};"
    + $"User Id={db_username};"
    + "Encrypt=false;"
    + "MultipleActiveResultSets=true;"
    + "TrustServerCertificate=true;";
var db_password = $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

Console.WriteLine($"Connecting to database via {connectionString} "
        + $"with a password of {Environment.GetEnvironmentVariable("DB_PASSWORD")?.Length} chars long.");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(connectionString + db_password));

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

    try
    {
        var canConnect = await db.Database.ExecuteSqlRawAsync("SELECT 1");
        app.Logger.LogInformation("Successfully connected to database and executed test query.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to connect to database or execute query.");
    }
}

app.Run();


