using Octokit;

namespace WelcomeToOSD.Api.DTOs
{
    public record IssueDto()
    {
        public long Id { get; init; }
        public int Number { get; init; }
        public string? HtmlUrl { get; init; }
        public string? State { get; init; }
        public string? Title { get; init; }
        public string? Body { get; init; }
        public PullRequestDto? PullRequest { get; init; }
        public int Comments { get; init; }
        public IEnumerable<LabelDto>? Labels { get; init; }
    }

    public record PullRequestDto
    {
        public string? HtmlUrl { get; init; }
    }

    public record LabelDto
    {
        public long Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
    }
}