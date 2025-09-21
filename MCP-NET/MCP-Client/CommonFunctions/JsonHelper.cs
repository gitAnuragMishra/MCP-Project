using System.Text;
using System.Text.Json;

namespace CommonFunctions
{
    public class JsonHelper
    {
        public static string ExtractFirstJsonObject(string text)
        {
            int start = text.IndexOf('{');
            if (start == -1) return null;

            int depth = 0;
            bool inString = false;
            StringBuilder sb = new StringBuilder();

            for (int i = start; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"' && (i == 0 || text[i - 1] != '\\'))
                    inString = !inString;

                if (!inString)
                {
                    if (c == '{') depth++;
                    else if (c == '}') depth--;
                }

                sb.Append(c);

                if (depth == 0 && !inString)
                    break;
            }

            return sb.ToString();
        }

        public static string Serialize<T>(T model)
        {
            var jsonOpts = new JsonSerializerOptions { WriteIndented = true };
            string result = JsonSerializer.Serialize(model, jsonOpts);

            return result;
        }
    }
}
