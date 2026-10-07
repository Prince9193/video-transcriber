using System.Text.Json;

namespace VideoGateway.Services;

// Typed HttpClient that talks to the existing Python/Flask pipeline. The Flask code is untouched.
public class FlaskClient
{
    private readonly HttpClient _http;

    public FlaskClient(HttpClient http) => _http = http;

    public async Task<string> TranslateAsync(string inputPath, string language, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        await using var fileStream = File.OpenRead(inputPath);
        form.Add(new StreamContent(fileStream), "video", Path.GetFileName(inputPath));
        form.Add(new StringContent(language), "language");

        using var response = await _http.PostAsync("/api/translate", form, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        var status = root.TryGetProperty("status", out var s) ? s.GetString() : null;

        if (!response.IsSuccessStatusCode || status != "success")
        {
            var message = root.TryGetProperty("message", out var m) ? m.GetString() : "Flask pipeline failed";
            throw new InvalidOperationException(message);
        }

        return root.GetProperty("output_file").GetString()
               ?? throw new InvalidOperationException("Flask returned no output file name");
    }

    public async Task<string> DownloadToAsync(string flaskFileName, string destinationPath, CancellationToken ct)
    {
        using var response = await _http.GetAsync(
            $"/api/download/{Uri.EscapeDataString(flaskFileName)}", HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var source = await response.Content.ReadAsStreamAsync(ct);
        await using var target = File.Create(destinationPath);
        await source.CopyToAsync(target, ct);
        return destinationPath;
    }

    public async Task<string> GetLanguagesJsonAsync(CancellationToken ct)
    {
        using var response = await _http.GetAsync("/api/languages", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }
}
