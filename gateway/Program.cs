using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using VideoGateway.Services;

var builder = WebApplication.CreateBuilder(args);

var maxUploadBytes = builder.Configuration.GetValue("Storage:MaxUploadMegabytes", 100) * 1024L * 1024L;

builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = maxUploadBytes + 1024 * 1024);
builder.Services.Configure<KestrelServerOptions>(o => o.Limits.MaxRequestBodySize = maxUploadBytes + 1024 * 1024);

builder.Services.AddControllers();

// Dependency injection: singletons hold shared state, the worker runs for the app's lifetime.
builder.Services.AddSingleton<IJobStore, InMemoryJobStore>();
builder.Services.AddSingleton<JobQueue>();
builder.Services.AddSingleton<StoragePaths>();
builder.Services.AddHttpClient<FlaskClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Flask:BaseUrl"] ?? "http://localhost:5000");
    client.Timeout = TimeSpan.FromMinutes(builder.Configuration.GetValue("Flask:TimeoutMinutes", 30));
});
builder.Services.AddHostedService<TranslationWorker>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();

public partial class Program { }
