using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace TEST.Webhook.Application;

/// <summary>
/// Module for the Application layer, configuring dependencies and services.
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpCachingModule)
)]
public class ApiApplicationModule : AbpModule
{
    /// <inheritdoc/>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        services.AddAutoMapperObjectMapper<ApiApplicationModule>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<ApiApplicationModule>(); });
    }
}