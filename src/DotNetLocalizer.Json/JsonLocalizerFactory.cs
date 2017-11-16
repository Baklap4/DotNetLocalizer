using System;
using DotNetLocalizer.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LocalizationOptions = DotNetLocalizer.Core.LocalizationOptions;

namespace DotNetLocalizer.Json
{
    public class JsonLocalizerFactory : BaseLocalizerFactory
    {
        public JsonLocalizerFactory(IHostingEnvironment applicationEnvironment, IOptions<LocalizationOptions> options, ILoggerFactory loggerFactory) : base(applicationEnvironment, options, loggerFactory)
        {
        }

        public override IStringLocalizer Create(Type resourceSource)
        {
            this.logger.LogInformation($"Getting localizer for type {resourceSource}");
            return this.localizerCache.GetOrAdd(this.resourceLocation, new JsonLocalizer(this.resourceLocation, this.logger));
        }

        public override IStringLocalizer Create(string baseName, string location)
        {
            this.logger.LogInformation($"Getting localizer for basename: {baseName}");
            return this.localizerCache.GetOrAdd(this.resourceLocation, new JsonLocalizer(this.resourceLocation, this.logger));
        }
    }
}