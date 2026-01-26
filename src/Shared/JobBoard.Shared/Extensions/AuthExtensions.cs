using System.Net;
using System.Text;
using System.Text.Json;
using JobBoard.Shared.ApiResponse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Shared.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJobBoardAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:SecretKey"];
        var issuer = configuration["JwtSettings:Issuer"];
        var audience = configuration["JwtSettings:Audience"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException("JwtSettings:SecretKey", "JWT Secret Key is missing in configuration");
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    
                    ClockSkew = TimeSpan.Zero 
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        var apiResponse = new ApiErrorResponse(
                            "Unauthorized", 
                            "Full authentication is required to access this resource."
                        );

                        var json = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions 
                        { 
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                        });

                        return context.Response.WriteAsync(json);
                    },
                    
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";

                        var apiResponse = new ApiErrorResponse(
                            "Forbidden", 
                            "You do not have permission to access this resource."
                        );

                        var json = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions 
                        { 
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                        });

                        return context.Response.WriteAsync(json);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}