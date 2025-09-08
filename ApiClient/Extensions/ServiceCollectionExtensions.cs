using ApiClient.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    private const string AuthorizationCodeDefinitionName = "AuthorizationCode";
    private const string BearerDefinitionName = "Bearer";

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<AuthenticationConfig>()
            .BindConfiguration(AuthenticationConfig.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddAndConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authConfig = GetAuthenticationConfig(configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.Audience = authConfig.Audience;
                o.MetadataAddress = authConfig.MetadataAddress;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authConfig.ValidIssuer
                };
            });

        return services;
    }

    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authConfig = GetAuthenticationConfig(configuration);

        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition(AuthorizationCodeDefinitionName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authConfig.AuthUrl),
                        TokenUrl = new Uri(authConfig.TokenUrl),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "profile", "Profile" }
                        }
                    }
                }
            });

            o.AddSecurityDefinition(BearerDefinitionName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = AuthorizationCodeDefinitionName,
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = BearerDefinitionName,
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                }
            });
        });


        return services;
    }

    private static AuthenticationConfig GetAuthenticationConfig(IConfiguration configuration)
    {
        var authConfig = configuration.GetSection(AuthenticationConfig.SectionName).Get<AuthenticationConfig>();
        return authConfig ?? throw new InvalidOperationException("Authentication configuration is not found");
    }
}