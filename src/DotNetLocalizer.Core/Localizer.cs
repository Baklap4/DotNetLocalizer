using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DotNetLocalizer.Core
{
    public abstract class Localizer : IStringLocalizer
    {
        protected readonly string fileLocation;
        protected readonly ILogger logger;
        protected readonly ConcurrentDictionary<string, Lazy<object>> resourceObjectCache = new ConcurrentDictionary<string, Lazy<object>>();

        protected Localizer(string fileLocation, ILogger logger)
        {
            this.fileLocation = fileLocation;
            this.logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => this.GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);
        public IStringLocalizer WithCulture(CultureInfo culture) => this.WithCultureSpecific(culture);

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected abstract IStringLocalizer WithCultureSpecific(CultureInfo culture);

        protected abstract string GetLocalizedString(string name, CultureInfo culture);

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
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, format == null);
            }
        }
    }
}