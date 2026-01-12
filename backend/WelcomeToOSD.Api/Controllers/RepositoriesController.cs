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
    public async Task<IActionResult> GetRepositories(int page = 1, int perPage = 20)
    {
        var beginnerReposRequest = new SearchRepositoriesRequest("is:public archived:false good-first-issues:>0")
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending,
            Page = page,
            PerPage = perPage
        };

        var highStarredReposRequest = new SearchRepositoriesRequest("is:public archived:false")
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending
        };

        // There is an issue here with not fetching all at once. Now when we get page 2, they aren't completely sorted
        // meaning that the second list will have higher stargzers count than the last item in the first list.
        // to fix this we need to fetch ALL (up to the Github 1000 repo) limit.
        var beginnerRepoResultsTask = _httpClient.Search.SearchRepo(beginnerReposRequest);
        var highStarredReposResultsTask = _httpClient.Search.SearchRepo(highStarredReposRequest);
        await Task.WhenAll(beginnerRepoResultsTask, highStarredReposResultsTask);

        var beginnerResults = beginnerRepoResultsTask.Result.Items;
        var highStarredResults = highStarredReposResultsTask.Result.Items;

        var mergedResults = beginnerResults
            .Concat(highStarredResults)
            .GroupBy(repository => repository.FullName)
            .Distinct()
            .Select(repository => repository.FirstOrDefault())
            .OrderByDescending(repository => repository?.StargazersCount)
            .ToList();

        return Ok(mergedResults);
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

    [HttpGet("{owner}/{repositoryName}/contents/{filePath}")]
    public async Task<IActionResult> GetFileContents(string owner, string repositoryName, string filePath)
    {
        var fileContent = await _httpClient.Repository.Content.GetRawContent(owner, repositoryName, filePath);
        return Ok();
    }
}