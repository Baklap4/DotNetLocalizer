using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotNetLocalizer.Json.Extensions
{
    public static class JObjectExtensions
    {
        public static bool TryGetValue(this JObject json, string query, out JToken token, bool nested)
        {
            token = null;
            if (string.IsNullOrEmpty(query))
            {
                return false;
            }
            if (!nested)
            {
                if (!json.TryGetValue(query, out JToken value))
                {
                    return false;
                }
                token = value;
                return true;
            }
            token = json;
            return JObjectExtensions.HasValue(query, ref token, true);
        }

        private static bool HasValue(string query, ref JToken token, bool hasValue)
        {
            foreach (var queryComponent in query.Split('.'))
            {
                if (int.TryParse(queryComponent, out int index))
                {
                    if (JObjectExtensions.IsIndexInRangeAndHasValue(ref token, out hasValue, index))
                    {
                        continue;
                    }
                    break;
                }
                token = token[queryComponent];
                if (token != null)
                {
                    continue;
                }
                hasValue = false;
                break;
            }
            return hasValue;
        }

        private static bool IsIndexInRangeAndHasValue(ref JToken token, out bool hasValue, int index)
        {
            if (index >= token.Count())
            {
                return hasValue = false;
            }
            token = token[index];
            if (token != null)
            {
                return hasValue = true;
            }
            return hasValue = false;
        }
    }
}