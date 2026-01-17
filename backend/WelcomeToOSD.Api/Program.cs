using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Octokit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

builder.Services.AddOutputCache();

// Register GitHubClient as a singleton for DI
// Don't need any PAT or OIDC
builder.Services.AddSingleton<GitHubClient>(sp =>
    {
        var client = new GitHubClient(new Octokit.ProductHeaderValue("WelcomeToOSD"))
        {
            Credentials = new Credentials(
                Environment.GetEnvironmentVariable("GITHUB_TOKEN")
            )
        };
        
        return client;
    }
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxt", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Beaer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowNuxt");

app.UseOutputCache();
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
