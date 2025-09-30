using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Veiling.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Create connection string
string? db_server;
if (builder.Configuration["Docker:IsContainerized"] == "False") {
    Env.TraversePath().Load();
    db_server = Environment.GetEnvironmentVariable("DB_SERVER");
} else {
    db_server = "tcp:db,1433";
}
var connectionString
    = $"Server={db_server};"
    + $"Database={Environment.GetEnvironmentVariable("DB_NAME")};"
    + $"User Id={Environment.GetEnvironmentVariable("DB_USERNAME")};"
    + $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};"
    + "Encrypt=false;"
    + "MultipleActiveResultSets=true;"
    + "TrustServerCertificate=true";

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

    if (db.Database.CanConnect())
    {
        app.Logger.LogInformation("Successfully connected to the database!");
    }
    else
    {
        app.Logger.LogError("Failed to connect to the database!");
    }
}

app.Run();


