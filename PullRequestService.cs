using Octokit;
using System.Threading.Tasks;

public class PullRequestService
{
    private readonly string token = Config.Get("GitHubToken");
    private readonly string owner = Config.Get("RepoOwner");
    private readonly string repo = Config.Get("RepoName");

    public async Task CreatePullRequest(string branchName, string title, string body)
    {
        var client = new GitHubClient(new ProductHeaderValue("postman-sync"));
        client.Credentials = new Credentials(token);

        await client.PullRequest.Create(owner, repo, new NewPullRequest(title, branchName, "main") { Body = body });
    }
}
