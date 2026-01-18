using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Veiling.Server;
using Veiling.Server.Models;

namespace Veiling.Server.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private static bool _rolesInitialized = false;
        private static readonly object _lock = new object();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Disable authorization for tests
                services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
                
                // Initialize roles for testing - only once
                lock (_lock)
                {
                    if (!_rolesInitialized)
                    {
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        InitializeRoles(roleManager).Wait();
                        _rolesInitialized = true;
                    }
                }
            });
        }
        
        private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create all roles from the Role enum
            foreach (var roleName in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }

    // Authorization handler that allows all requests in test environment
    public class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}