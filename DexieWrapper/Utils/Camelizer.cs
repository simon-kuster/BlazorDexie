namespace Nosthy.Blazor.DexieWrapper.Utils
{
    public static class Camelizer
    {
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            char firstChar = str[0];

            if (firstChar == '[')
            {
                var subStrings = str.TrimStart('[').TrimEnd(']').Split('+');
                var camelizedSubStrings = subStrings.Select(s => ToCamelCase(s.Trim())).ToArray();
                return "[" + string.Join("+", camelizedSubStrings) + "]";
            }

            if (firstChar == '&' || firstChar == '*')
            {
                return firstChar + ToCamelCase(str.Substring(1));
            }

            if (str.StartsWith("++"))
            {
                return "++" + ToCamelCase(str.Substring(2));
            }

            return str[0].ToString().ToLower() + str.Substring(1);
        }
    }
}
