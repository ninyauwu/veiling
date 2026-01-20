using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Veiling.Server.Models;

namespace Veiling.Server.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/me", async (
            ClaimsPrincipal user,
            UserManager<Gebruiker> userManager
        ) =>
        {
            var appUser = await userManager.GetUserAsync(user);

            if (appUser == null)
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(appUser);

            return Results.Ok(new
            {
                email = appUser.Email,
                isEmailConfirmed = appUser.EmailConfirmed,
                roles
            });
        })
        .RequireAuthorization();
    }
}
