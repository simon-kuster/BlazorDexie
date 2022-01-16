namespace DexieWrapper.Utils
{
    public static class Camelizer
    {
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str[0].ToString().ToLower() + str.Substring(1);
            }
        }
    }
}
