using System;
using System.Globalization;
using DotNetLocalizer.Core;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DotNetLocalizer.Yaml
{
    public class YamlLocalizer : Localizer
    {
        public YamlLocalizer(string fileLocation, ILogger logger) : base(fileLocation, logger)
        {
        }

        protected override IStringLocalizer WithCultureSpecific(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override string GetLocalizedString(string name, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}