using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetLocalizer.Core
{
    public abstract class BaseLocalizerFactory : IStringLocalizerFactory
    {
        protected readonly ConcurrentDictionary<string, Localizer> localizerCache = new ConcurrentDictionary<string, Localizer>();
        protected readonly ILogger logger;
        protected readonly string resourceLocation;

        protected BaseLocalizerFactory(IHostingEnvironment applicationEnvironment,
                                       IOptions<LocalizationOptions> options,
                                       ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<BaseLocalizerFactory>();

            if (!string.IsNullOrEmpty(options.Value.ResourcesPath) && Directory.Exists(options.Value.ResourcesPath))
            {
                this.resourceLocation = Path.Combine(applicationEnvironment.ContentRootPath, options.Value.ResourcesPath) + Path.DirectorySeparatorChar;
            }
            else
            {
                this.resourceLocation = Path.Combine(applicationEnvironment.ContentRootPath, "Localization") + Path.DirectorySeparatorChar;
                this.logger.LogInformation($"No resourcepath was configured. Falling back to default location: {Path.Combine(applicationEnvironment.ContentRootPath, this.resourceLocation) + Path.DirectorySeparatorChar}");
            }
        }

        public abstract IStringLocalizer Create(Type resourceSource);
        public abstract IStringLocalizer Create(string baseName, string location);
        
    }
}