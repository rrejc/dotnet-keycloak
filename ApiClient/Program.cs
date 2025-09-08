using System.Security.Claims;
using ApiClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguration();
builder.Services.AddAuthorization();
builder.Services.AddAndConfigureAuthentication(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => { options.OAuthUsePkce(); });

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/get-claims",
        (ClaimsPrincipal claimsPrincipal) =>
        {
            return claimsPrincipal.Claims.ToDictionary(cp => cp.Type, cp => cp.Value);
        })
    .RequireAuthorization();

app.Run();