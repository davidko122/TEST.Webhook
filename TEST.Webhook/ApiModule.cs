using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using TEST.Webhook.Application;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace TEST.Webhook;


[DependsOn(
    typeof(AbpAspNetCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
  //  typeof(AbpAutoMapperModule),
    typeof(ApiApplicationModule),
    typeof(AbpAspNetCoreSignalRModule)


)]
public class ApiModule : AbpModule
{

    private static void ConfigureAuth(IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("ConfigureAuth 1");
       
        Log.Information("ConfigureAuth done");
    }

    private static void ConfigureSwaggerServices(IServiceCollection services)
    {
        Log.Information("ConfigureSwaggerServices 1");
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "KOG CRM API", Version = "v3" });
                options.DocInclusionPredicate((docName, description) => true);
                options.DocumentFilter<CustomSwaggerFilter>();
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "KOG.CRM.HttpApi.xml"));
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "KOG.CRM.Application.xml"));
            }
        );
        Log.Information("ConfigureSwaggerServices done");

    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Information("ConfigureServices 1");

        var services = context.Services;
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        // Register IHttpClientFactory for services using HttpClient.
        services.AddHttpClient();
        //services.AddRazorComponents().AddInteractiveServerComponents();
        Configure<AbpAspNetCoreMvcOptions>(options => { options.ConventionalControllers.Create(typeof(ApiApplicationModule).Assembly); });
       // Configure<AbpAutoMapperOptions>(options => { options.AddMaps<ApiModule>(); });

        services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
      //  services.AddHostedService<TokenBackgroundService>();

        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "KOGAPI:"; });
        ConfigureAuth(services, configuration);

        if (hostingEnvironment.IsDevelopment() || hostingEnvironment.IsStaging())
        {
            ConfigureSwaggerServices(services);
            context.Services.AddCors(options => { options.AddDefaultPolicy(builder => { builder.WithOrigins("http://localhost:3000").WithAbpExposedHeaders().SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader().AllowAnyMethod().AllowCredentials(); }); });
        }
        Log.Information("ConfigureServices done");


    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        app.UseRouting();
        //app.UseMiddleware<RequestLoggingMiddleware>();
        if (env.IsDevelopment() || env.IsStaging())
        {
            app.UseCors();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                options.DefaultModelsExpandDepth(-1);
            });
        }
        //app.UseAuthentication();
        //app.UseAuthorization();
        app.UseConfiguredEndpoints(endpoints =>
        {
           // endpoints.MapHub<NotificationHub>("/crm-messaging-hub", options => { options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30); });
        });
    }

    public class CustomSwaggerFilter : IDocumentFilter
    {

        /// <inheritdoc/>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Where(x => x.Key.ToLowerInvariant().StartsWith("/api/abp", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => swaggerDoc.Paths.Remove(x.Key));
        }
    }

}
