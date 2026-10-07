namespace VideoGateway.Services;

public sealed class StoragePaths
{
    public StoragePaths(IConfiguration configuration, IHostEnvironment environment)
    {
        UploadFolder = Resolve(configuration["Storage:UploadFolder"], environment.ContentRootPath, "uploads");
        OutputFolder = Resolve(configuration["Storage:OutputFolder"], environment.ContentRootPath, "outputs");
    }

    public string UploadFolder { get; }
    public string OutputFolder { get; }

    private static string Resolve(string? configuredPath, string contentRoot, string fallback)
    {
        var path = string.IsNullOrWhiteSpace(configuredPath) ? fallback : configuredPath;
        return Path.GetFullPath(Path.IsPathFullyQualified(path)
            ? path
            : Path.Combine(contentRoot, path));
    }
}
