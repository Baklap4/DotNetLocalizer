using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DotNetLocalizer.Core;
using DotNetLocalizer.Json.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetLocalizer.Json
{
    public class JsonLocalizer : Localizer
    {
        public JsonLocalizer(string fileLocation, ILogger logger) : base(fileLocation, logger)
        {
        }

        protected override IStringLocalizer WithCultureSpecific(CultureInfo culture)
        {
            if (culture == null)
            {
                return new JsonLocalizer(this.fileLocation, this.logger);
            }
            throw new NotImplementedException();
        }

        protected override string GetLocalizedString(string name, CultureInfo culture)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            CultureInfo previousCulture = null;

            while (!Object.Equals(previousCulture, currentCulture) && !String.IsNullOrEmpty(currentCulture.TwoLetterISOLanguageName) && currentCulture.TwoLetterISOLanguageName != "iv")
            {
                var resourceObject = this.GetResourceObject(currentCulture);
                if (resourceObject == null)
                {
                    this.logger.LogWarning($"No resource file found or error occurred, culture {currentCulture} and key '{name}'");
                }
                else
                {
                    if (resourceObject.TryGetValue(name, out var foundLocalizedValue, name.Contains('.')))
                    {
                        return foundLocalizedValue.ToString();
                    }
                }

                // Consult parent culture.
                previousCulture = currentCulture;
                currentCulture = currentCulture.Parent;
                this.logger.LogTrace($"Switching to parent culture {currentCulture} for key '{name}'.");
            }

            this.logger.LogWarning($"Could not find key '{name}' in resource file and culture {CultureInfo.CurrentCulture}");
            if (!this.missingResources.ContainsKey(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
            {
                this.missingResources.Add(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, new List<string>());
            }
            if (this.missingResources[CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Any(x => x == name))
            {
                return null;
            }
            this.missingResources[CultureInfo.CurrentCulture.TwoLetterISOLanguageName].Add(name);
            File.WriteAllLines(Path.Combine(this.fileLocation, $"MissingStrings.{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.json"), this.missingResources[CultureInfo.CurrentCulture.TwoLetterISOLanguageName]);
            return null;
        }

        private JObject GetResourceObject(CultureInfo currentCulture)
        {
            this.logger.LogTrace($"Attempt to get resource object for culture {currentCulture}");
            var cultureSuffix = "." + currentCulture.Name;

            var lazyJObjectGetter = new Lazy<object>(() =>
                                                     {
                                                         var resourcePath = this.fileLocation + "Strings." + currentCulture.TwoLetterISOLanguageName + ".json";
                                                         try
                                                         {
                                                             var resourceFileStream =
                                                                 new FileStream(resourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                                                             using (resourceFileStream)
                                                             {
                                                                 var resourceReader =
                                                                     new JsonTextReader(new StreamReader(resourceFileStream, Encoding.UTF8, true));
                                                                 using (resourceReader)
                                                                 {
                                                                     return JObject.Load(resourceReader);
                                                                 }
                                                             }
                                                         }
                                                         catch (Exception e)
                                                         {
                                                             this.logger.LogError($"Error occurred attempting to read JSON resource file {resourcePath}: {e}");
                                                             return null;
                                                         }
                                                     },
                                                     LazyThreadSafetyMode.ExecutionAndPublication);

            lazyJObjectGetter = this.resourceObjectCache.GetOrAdd(cultureSuffix, lazyJObjectGetter);
            var resourceObject = JObject.FromObject(lazyJObjectGetter.Value);
            return resourceObject;
        }
    }
}