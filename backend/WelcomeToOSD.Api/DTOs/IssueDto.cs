using Octokit;

namespace WelcomeToOSD.Api.DTOs
{
    public record IssueDto()
    {
        public required long Id { get; init; }
        public required int Number { get; init; }
        public required string? HtmlUrl { get; init; }
        public required string? State { get; init; }
        public required string? Title { get; init; }
        public required string? Body { get; init; }
        public required PullRequestDto? PullRequest { get; init; }
        public required int Comments { get; init; }
        public required IEnumerable<LabelDto>? Labels { get; init; }
    }

    public record PullRequestDto
    {
        public required string? HtmlUrl { get; init; }
    }

    public record LabelDto
    {
        public required long Id { get; init; }
        public required string? Name { get; init; }
        public required string? Description { get; init; }
    }
}