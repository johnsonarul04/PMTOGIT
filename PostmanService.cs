using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

public class PostmanService
{
    private readonly HttpClient _client = new HttpClient();
    private readonly string _apiKey = Config.Get("PostmanApiKey");
    private readonly string _workspaceId = Config.Get("WorkspaceId");

    public async Task<string> GetCollectionIdByName(string name)
    {
        var url = $"https://api.postman.com/collections?workspace={_workspaceId}";
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

        var response = await _client.GetStringAsync(url);
        using var doc = JsonDocument.Parse(response);
        foreach (var col in doc.RootElement.GetProperty("collections").EnumerateArray())
        {
            if (col.GetProperty("name").GetString() == name)
                return col.GetProperty("uid").GetString();
        }

        throw new Exception($"Collection '{name}' not found.");
    }

    public async Task<string> DownloadCollectionJson(string id, string name)
    {
        var url = $"https://api.postman.com/collections/{id}";
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

        var json = await _client.GetStringAsync(url);

        Directory.CreateDirectory("export");
        string filePath = $"export/{name}.json";
        await File.WriteAllTextAsync(filePath, json);

        return filePath;
    }
}
