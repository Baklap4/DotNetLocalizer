using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DotNetLocalizer.Core
{
    public abstract class Localizer : IStringLocalizer
    {
        protected readonly string fileLocation;
        protected readonly ILogger logger;
        protected readonly ConcurrentDictionary<string, Lazy<object>> resourceObjectCache = new ConcurrentDictionary<string, Lazy<object>>();
        protected readonly IDictionary<string, ICollection<string>> missingResources = new Dictionary<string, ICollection<string>>();

        protected Localizer(string fileLocation, ILogger logger)
        {
            this.fileLocation = fileLocation;
            this.logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.WithCultureSpecific(culture);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        protected IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures, CultureInfo culture)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            throw new NotImplementedException();
        }

        protected abstract IStringLocalizer WithCultureSpecific(CultureInfo culture);

        protected abstract string GetLocalizedString(string name, CultureInfo culture);

        public FileStream GetMissingLocalizedStrings(CultureInfo culture)
        {
            var path = Path.Combine(this.fileLocation, $"Missing.{culture.TwoLetterISOLanguageName}.json");
            return File.Exists(path) ? File.OpenRead(path) : null;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = this.GetLocalizedString(name, CultureInfo.CurrentUICulture);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = this.GetLocalizedString(name, CultureInfo.CurrentUICulture);
                var value = String.Format(format ?? name, arguments);
                return new LocalizedString(name, value, format == null);
            }
        }
    }
}