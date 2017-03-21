using System.Linq;

namespace Sfa.Roatp.Indexer.ApplicationServices.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCaseToSentence(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str[0] + string.Concat(str.Skip(1).Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString())).ToLower();
            }

            return string.Empty;
        }
    }
}