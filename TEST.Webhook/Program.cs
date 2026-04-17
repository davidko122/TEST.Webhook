using Serilog;
using TEST.Webhook;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext()
    .Filter.ByExcluding(logEvent =>
    {
        if (logEvent.Properties.TryGetValue("RequestPath", out var pathValue))
        {
            var path = pathValue.ToString();
            if (path.Contains("/crm-messaging-hub")) return true;
        }
        return false;
    })
    .WriteTo.Async(c => c.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 5 * 1024 * 1024,
        rollOnFileSizeLimit: true,
        shared: true,
        retainedFileCountLimit: 1000,
        retainedFileTimeLimit: TimeSpan.FromDays(90),
        flushToDiskInterval: TimeSpan.FromSeconds(5)
    )).WriteTo.Async(c => c.Console()).CreateLogger();
Log.Information("--Starting KOG CRM API 3--");


try
{
    builder.Host.UseAutofac().UseSerilog();
    await builder.AddApplicationAsync<ApiModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    if (ex is HostAbortedException) throw;
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally { Log.CloseAndFlush(); }