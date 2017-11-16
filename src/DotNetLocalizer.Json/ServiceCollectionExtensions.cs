using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using CoreServiceCollectionExtensions = DotNetLocalizer.Core.ServiceCollectionExtensions;
using LocalizationOptions = DotNetLocalizer.Core.LocalizationOptions;

namespace DotNetLocalizer.Json
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            return CoreServiceCollectionExtensions.AddLocalization(services)
                                                  .AddJsonLocalization(null);
        }

        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<LocalizationOptions> setupAction)
        {
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizerFactory), typeof(JsonLocalizerFactory), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizer), typeof(JsonLocalizer), ServiceLifetime.Singleton));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
            return services;
        }
    }
}
