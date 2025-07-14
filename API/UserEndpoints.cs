using System.Security.Claims;
using FluentValidation;
using Users.API.Application;
using Users.API.Enums;

namespace Users.API.API;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users");

        users.MapPost(
            "/register",
            async (RegisterRequest req, UserService svc, IValidator<RegisterRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(req);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                await svc.RegisterAsync(req.Email, req.Password, req.Role);
                return Results.Ok("Usuario registrado");
            }
        );

        users.MapPost(
            "/login",
            async (LoginRequest req, UserService svc, IValidator<LoginRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(req);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var token = await svc.LoginAsync(req.Email, req.Password);
                return Results.Ok(new AuthResponse(token));
            }
        );

        users
            .MapGet(
                "/me",
                async (UserService svc, ClaimsPrincipal userClaims) =>
                {
                    var user = await svc.GetCurrentUserAsync(userClaims);
                    return Results.Ok(new UserResponse(user.Email, user.Role));
                }
            )
            .RequireAuthorization();
    }
}

public record RegisterRequest(string Email, string Password, UserRole Role);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token);

public record UserResponse(string Email, UserRole Role);
