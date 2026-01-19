using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Veiling.Server;
using Microsoft.AspNetCore.Identity;
using Veiling.Server.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with Bearer token support
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();

    // Add bearer token authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the bearer token from the /login or /register endpoint"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext - Only configure SQL Server when not in Testing environment
if (!builder.Environment.IsEnvironment("Testing"))
{
    // ==Create connection string== (MOVED INSIDE THIS BLOCK)
    var containerized = builder.Configuration["Docker:IsContainerized"] != "False";
    if (!containerized)
    {
        Env.TraversePath().Load();
    }

    var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    var connectionString
        = builder.Configuration.GetConnectionString("Default")
        + $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};"
        + $"User Id={Environment.GetEnvironmentVariable("DB_USERNAME")};";

    Console.WriteLine($"Connecting to database via {connectionString}");

    if (password?.Length < 8) Console.WriteLine("Warning: Password should be at least 8 characters.");

    connectionString += $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Configure MVC
builder.Services.AddControllers(options =>
{
    // always clear filters in Testing environment
    if (builder.Environment.IsEnvironment("Testing"))
    {
        // Remove authorization filters for tests
        options.Filters.Clear();
    }
});

// Always register IAppDbContext interface
builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

// Identity - Configure authentication properly
builder.Services.AddAuthentication(options =>
{
    // Set Bearer as the default scheme for API endpoints
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
})
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<Gebruiker>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddApiEndpoints();

var app = builder.Build();

// Static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Dev Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapIdentityApi<Gebruiker>();
app.MapFallbackToFile("/index.html");

// DB seed + test - Only when not in Testing environment
if (!app.Environment.IsEnvironment("Testing"))
{
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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Gebruiker>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await AppDbSeeder.Seed(db, userManager, roleManager);
        }
    }
}

app.Run();

public partial class Program { }