using System;
using System.Diagnostics;
using System.IO;

public class GitService
{
    public string CommitFileToNewBranch(string filePath, string name)
    {
        string branchName = $"feature/{name}-update-{DateTime.UtcNow:yyyyMMddHHmm}";

        Run("git", $"checkout -b {branchName}");
        Run("git", $"add {filePath}");
        Run("git", $"commit -m \"Update {name} collection\"");
        Run("git", $"push origin {branchName}");

        return branchName;
    }

    private void Run(string cmd, string args)
    {
        var p = Process.Start(new ProcessStartInfo(cmd, args) { RedirectStandardOutput = true });
        p.WaitForExit();
    }
}
