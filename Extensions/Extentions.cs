namespace SolrLib.Extensions
{
    public static class Extentions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            return str;
            //var sb = new StringBuilder();
            //if (str != null)
            //{
            //    foreach (char c in str)
            //    {
            //        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ' || c == '\\' || c == '-')
            //        {
            //            sb.Append(c);
            //        }
            //    }
            //    return sb.ToString();
            //}

            //return string.Empty;

        }
    }
}
