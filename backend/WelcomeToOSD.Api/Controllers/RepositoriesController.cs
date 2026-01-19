using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Octokit;
using WelcomeToOSD.Api.DTOs;

namespace WelcomeToOSD.Api.Controllers;

// TODO: Need to create DTOs for these returns because Octokit isn't meant to be 
// explicitly returned without mapping. An example being the Issues API.

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
    [OutputCache(Duration = 600)]
    public async Task<IActionResult> GetFileContents(string owner, string repositoryName, string filePath)
    {
        var fileContent = await _httpClient.Repository.Content.GetRawContent(owner, repositoryName, filePath);
        var actualContent = Encoding.UTF8.GetString(fileContent);

        return Ok(actualContent);
    }

    [HttpGet("{owner}/{repositoryName}/languages")]
    [OutputCache(Duration = 600)]
    public async Task<IActionResult> GetRepositoryLanguages(string owner, string repositoryName)
    {
        var languages = await _httpClient.Repository.GetAllLanguages(owner, repositoryName);
        return Ok(languages);
    }

    [HttpGet("{owner}/{repositoryName}/issues")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetRepositoryIssues(string owner, string repositoryName)
    {
        try
        {
            var issues = await _httpClient.Issue.GetAllForRepository(owner, repositoryName);
            var issuesDto = issues?.Select(issue => new IssueDto
            {            
                Id = issue.Id,
                Number = issue.Number,
                HtmlUrl = issue.HtmlUrl ?? "",
                State = issue.State.StringValue ?? "",
                Title = issue.Title ?? "",
                Body = issue.Body ?? "",
                PullRequest = issue.PullRequest != null 
                    ? new PullRequestDto { HtmlUrl = issue.PullRequest.HtmlUrl ?? "" } 
                    : null,
                Comments = issue.Comments,
                Labels = issue.Labels?.Select(label => 
                    new LabelDto 
                    { 
                        Description = label.Description ?? "",
                        Id = label.Id,
                        Name = label.Name ?? ""
                    })
            });

            return Ok(issuesDto);
        } 
        catch (Exception ex)
        {
           throw new Exception($"Exception occurred {ex.Message}"); 
        }

    }
}