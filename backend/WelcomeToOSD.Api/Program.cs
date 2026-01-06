using Octokit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

// Register GitHubClient as a singleton for DI
// Don't need any PAT or OIDC
builder.Services.AddSingleton<GitHubClient>(sp =>
    new GitHubClient(new Octokit.ProductHeaderValue("WelcomeToOSD"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
