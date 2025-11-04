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

import requests
import json
import sys

API_KEY = "YOUR_POSTMAN_API_KEY"
WORKSPACE_ID = "YOUR_WORKSPACE_ID"
COLLECTION_NAME = "Your Collection Name"

headers = {
    "X-Api-Key": API_KEY
}

# 1. Get list of collections in workspace
url = f"https://api.getpostman.com/collections?workspace={WORKSPACE_ID}"
resp = requests.get(url, headers=headers)

if resp.status_code != 200:
    print("Failed to list collections:", resp.text)
    sys.exit(1)

collections = resp.json().get("collections", [])

# 2. Find the collection by name
target = next((c for c in collections if c["name"] == COLLECTION_NAME), None)

if not target:
    print(f"Collection '{COLLECTION_NAME}' not found in workspace.")
    sys.exit(1)

collection_uid = target["uid"]

# 3. Download the collection JSON
download_url = f"https://api.getpostman.com/collections/{collection_uid}"
download_resp = requests.get(download_url, headers=headers)

if download_resp.status_code != 200:
    print("Failed to download collection:", download_resp.text)
    sys.exit(1)

data = download_resp.json()

# 4. Save to file
output_file = f"{COLLECTION_NAME.replace(' ', '_')}.json"
with open(output_file, "w") as f:
    json.dump(data, f, indent=2)

print(f"✅ Collection downloaded successfully: {output_file}")

