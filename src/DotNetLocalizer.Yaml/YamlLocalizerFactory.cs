using System;
using DotNetLocalizer.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LocalizationOptions = DotNetLocalizer.Core.LocalizationOptions;

namespace DotNetLocalizer.Yaml
{
    public class YamlLocalizerFactory : BaseLocalizerFactory
    {
        public YamlLocalizerFactory(IHostingEnvironment applicationEnvironment, IOptions<LocalizationOptions> options, ILoggerFactory loggerFactory) : base(applicationEnvironment, options, loggerFactory)
        {
        }

        public override IStringLocalizer Create(Type resourceSource)
        {
            throw new NotImplementedException();
        }

        public override IStringLocalizer Create(string baseName, string location)
        {
            throw new NotImplementedException();
        }
    }
}