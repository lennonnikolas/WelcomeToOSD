using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Octokit;

namespace WelcomeToOSD.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepositoriesController(GitHubClient httpClient) : ControllerBase
{
    private readonly GitHubClient _httpClient = httpClient;

    [HttpGet(Name = "GetAllRepositoriesByQuery")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetRepositories([FromQuery] int page, [FromQuery] int perPage, [FromQuery] string? query)
    {
        if (string.IsNullOrEmpty(query))
            return BadRequest(new { error = "Query is required for proper search"});

        var request = new SearchRepositoriesRequest(query)
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending,
            Page = page,
            PerPage = perPage
        };

        var result = await _httpClient.Search.SearchRepo(request);
        
        return Ok(result.Items);
    }

    [HttpGet("{owner}/{repositoryName}")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetRepositoryByOwnerAndName(string owner, string repositoryName)
    {
        if (string.IsNullOrEmpty(repositoryName))
        {
            return BadRequest($"Must provide a valid {nameof(repositoryName)}.");
        }

        if (string.IsNullOrEmpty(owner))
        {
            return BadRequest($"Must provide a valid {nameof(owner)}.");
        }

        var result = await _httpClient.Repository.Get(owner, repositoryName);

        return Ok(result);
    }

    [HttpGet("{owner}/{repositoryName}/contents")]
    [OutputCache(Duration = 600)]
    public async Task<IActionResult> GetRepositoryContents(string owner, string repositoryName)
    {
        if (string.IsNullOrEmpty(repositoryName))
        {
            return BadRequest($"Must provide a valid {nameof(repositoryName)}.");
        }

        if (string.IsNullOrEmpty(owner))
        {
            return BadRequest($"Must provide a valid {nameof(owner)}.");
        }

        var rootDirectoryResults = await _httpClient.Repository.Content.GetAllContents(owner, repositoryName);
        var githubPathResults = await _httpClient.Repository.Content.GetAllContents(owner, repositoryName, ".github");

        var fullResults = rootDirectoryResults.Concat(githubPathResults)
            .Where(x => 
                (x.Path?.Contains(".github", StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Name?.Contains("readme", StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Name?.Contains("contributing", StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Name?.Contains("conduct", StringComparison.OrdinalIgnoreCase) ?? false)
            );
        
        return Ok(fullResults);
    }
}