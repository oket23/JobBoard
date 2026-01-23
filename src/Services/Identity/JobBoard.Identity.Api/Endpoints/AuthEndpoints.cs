using System.Security.Claims; 
using JobBoard.Identity.Domain.Abstractions.Services; 
using JobBoard.Identity.Domain.Requests.Auth;
using JobBoard.Identity.Domain.Response.Auth;
using JobBoard.Shared.ApiResponse;
using JobBoard.Shared.Exceptions;
using JobBoard.Shared.Filters; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace JobBoard.Identity.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder UseAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Auth"); 
        
        group.MapPost("/login", async (
            [FromBody] LoginUserRequest request, 
            IAuthService service,
            CancellationToken ct) =>
            {
                var result = await service.Login(request, ct);
                return Results.Ok(new ApiSuccessResponse<AuthResponse>(result, "Login successful"));
            })
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidationEndpointFilter<LoginUserRequest>>();
        
        group.MapPost("/register", async (
            [FromBody] RegisterUserRequest request, 
            IAuthService service, 
            CancellationToken ct) =>
            {
                var result = await service.Register(request, ct);
                return Results.Ok(new ApiSuccessResponse<AuthResponse>(result, "Registration successful"));
            })
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidationEndpointFilter<RegisterUserRequest>>();
        
        group.MapPost("/refresh", async (
            [FromBody] RefreshTokenRequest request, 
            IAuthService service, 
            CancellationToken ct) =>
            {
                var result = await service.RefreshToken(request, ct);
                return Results.Ok(new ApiSuccessResponse<AuthResponse>(result, "Token refreshed"));
            })
            .AllowAnonymous();
        
        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService service, CancellationToken ct) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                   ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                   ?? user.FindFirst("id")?.Value;
                
                if (!int.TryParse(userIdString, out var userId))
                {
                    throw new UnauthorizedException("Invalid token claims");
                }
                
                var result = await service.GetUserProfile(userId, ct);
                
                return Results.Ok(new ApiSuccessResponse<AuthResponse>(result, "User profile fetched"));
            })
            .RequireAuthorization(); 

        return app;
    }
}