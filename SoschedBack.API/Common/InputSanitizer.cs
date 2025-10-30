using System.Web;

namespace SoschedBack.Common;

public static class InputSanitizer
{
    public static string Sanitize(string input)
    {
        return string.IsNullOrEmpty(input) ? input : HttpUtility.HtmlEncode(input); // Basic HTML encoding
    }
}