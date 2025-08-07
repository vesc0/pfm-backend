using System.Text.Json;

namespace PFM.API.Serialization
{
    public class KebabCaseNamingPolicy : JsonNamingPolicy
    {
        public static readonly KebabCaseNamingPolicy Instance = new();

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    // add dash before uppercase (except first letter)
                    if (i > 0)
                        sb.Append('-');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}