using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetLocalizer.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalization(this IServiceCollection services)
        {
            return services.AddLocalization(null);
        }
        public static IServiceCollection AddLocalization(this IServiceCollection services, Action<LocalizationOptions> setupAction)
        {
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
            return services;
        }
    }
}