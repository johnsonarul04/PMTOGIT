using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string collectionName = args.Length > 0 ? args[0] : throw new Exception("Provide collection name");

        var postman = new PostmanService();
        var git = new GitService();
        var pr = new PullRequestService();

        // 1. Get Collection ID by Name
        string collectionId = await postman.GetCollectionIdByName(collectionName);

        // 2. Download JSON
        string exportPath = await postman.DownloadCollectionJson(collectionId, collectionName);

        // 3. Create new Git branch + add file
        string branchName = git.CommitFileToNewBranch(exportPath, collectionName);

        // 4. Create PR
        await pr.CreatePullRequest(branchName, $"Update {collectionName} Collection", "Automated Collection Update");

        Console.WriteLine("✅ Completed.");
    }
}
