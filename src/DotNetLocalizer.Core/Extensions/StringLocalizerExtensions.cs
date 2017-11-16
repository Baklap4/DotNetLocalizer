using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;

namespace DotNetLocalizer.Core.Extensions
{
    public static class StringLocalizerExtensions
    {
        private const string MissingString = "MissingStrings";
        private const string LocalizationFolder = "Localization";
        private const string Extension = "json";

        public static FileStream GetMissingLocalizedStrings(this IStringLocalizer localizer, string fileLocation = null, CultureInfo culture = null)
        {
            var path = string.IsNullOrEmpty(fileLocation) && culture == null ? Path.Combine(Directory.GetCurrentDirectory(), StringLocalizerExtensions.LocalizationFolder, $"{StringLocalizerExtensions.MissingString}.{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.{StringLocalizerExtensions.Extension}") : (string.IsNullOrEmpty(fileLocation) && culture != null ? Path.Combine(Directory.GetCurrentDirectory(), StringLocalizerExtensions.LocalizationFolder, $"{StringLocalizerExtensions.MissingString}.{culture.TwoLetterISOLanguageName}.{StringLocalizerExtensions.Extension}") : (!string.IsNullOrEmpty(fileLocation) && culture == null ? Path.Combine(fileLocation, $"{StringLocalizerExtensions.MissingString}.{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.{StringLocalizerExtensions.Extension}") : Path.Combine(fileLocation, $"{StringLocalizerExtensions.MissingString}.{culture?.TwoLetterISOLanguageName}.{StringLocalizerExtensions.Extension}")));
            return File.Exists(path) ? File.OpenRead(path) : null;
        }
    }
}
