using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using CoreServiceCollectionExtensions = DotNetLocalizer.Core.ServiceCollectionExtensions;
using LocalizationOptions = DotNetLocalizer.Core.LocalizationOptions;

namespace DotNetLocalizer.Yaml
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddYamlLocalization(this IServiceCollection services)
        {
            return CoreServiceCollectionExtensions.AddLocalization(services)
                                                  .AddYamlLocalization(null);
        }

        public static IServiceCollection AddYamlLocalization(this IServiceCollection services, Action<LocalizationOptions> setupAction)
        {
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizerFactory), typeof(YamlLocalizerFactory), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizer), typeof(YamlLocalizer), ServiceLifetime.Singleton));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
            return services;
        }
    }
}