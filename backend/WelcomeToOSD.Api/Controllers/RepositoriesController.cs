using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace WelcomeToOSD.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepositoriesController(GitHubClient httpClient) : ControllerBase
{
    private GitHubClient _httpClient = httpClient;

    [HttpGet(Name = "GetAllRepositoriesByQuery")]
    public async Task<IActionResult> GetRepositories(string? query)
    {
        if (string.IsNullOrEmpty(query))
            return BadRequest(new { error = "Query is required for proper search"});

        var request = new SearchRepositoriesRequest(query)
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending
        };

        var result = await _httpClient.Search.SearchRepo(request);
        return Ok(result.Items);
    }
}