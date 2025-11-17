using System.Text.RegularExpressions;

namespace SoschedBack.Core.Common.Regex;

public class RegexPatterns
{
    public static readonly Dictionary<Pattern, (System.Text.RegularExpressions.Regex Pattern, System.Text.RegularExpressions.Regex PostgresPattern, short MaxLength, string Description)> Patterns = new()
    {
        [Pattern.Title] = 
            (new System.Text.RegularExpressions.Regex(@"^[\p{L}\p{N}\s\-.,_&()]+$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^[[:alnum:]\s\-.,_&()]+$", RegexOptions.Compiled),
            100, 
            "Titles must contain letters, numbers, spaces, and punctuation like -.,_&()."),

        [Pattern.Name] = 
            (new System.Text.RegularExpressions.Regex(@"^\p{L}+(?:[\s'-]\p{L}+)*$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^[[:alpha:]]+(?:[\s'-][[:alpha:]]+)*$", RegexOptions.Compiled),
            100, 
            "Full names may include letters, apostrophes, hyphens, and spaces."),

        [Pattern.Address] = 
            (new System.Text.RegularExpressions.Regex(@"^[\p{L}\p{N}\s'.,#/()\-]+$", RegexOptions.Compiled), //TODO: '#' doesn't work
            new System.Text.RegularExpressions.Regex(@"^[[:alnum:]\s'.,#/()\-]+$", RegexOptions.Compiled), 
            200, 
            "Addresses may contain letters, numbers, spaces, and punctuation like '.,#/-()."),

        [Pattern.Email] = 
            (new System.Text.RegularExpressions.Regex(@"^(?!\.)[A-Za-z0-9._%+-]+(?<!\.)@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^(?!\.)[A-Za-z0-9._%+-]+(?<!\.)@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$", RegexOptions.Compiled), 
            100, 
            "Must be a valid email format like user@example.com."),
        
        [Pattern.Password] =
        (   new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", RegexOptions.Compiled),
            100,
            "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit."
        ),

        [Pattern.PhoneE164] = 
            (new System.Text.RegularExpressions.Regex(@"^\+[1-9]\d{6,14}$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^\+[1-9]\d{6,14}$", RegexOptions.Compiled), 
            15, 
            "Phone number must follow the E.164 format, e.g., +1234567890."),

        [Pattern.Domain] = 
            (new System.Text.RegularExpressions.Regex(@"^(?:[\p{L}0-9-]{1,63}\.)+[A-Za-z]{2,}$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^(?:[[:alpha:]0-9-]{1,63}\.)+[A-Za-z]{2,}$", RegexOptions.Compiled), 
            255, 
            "Domain must be valid and support international formats like sub.domain.com."),

        [Pattern.Description] = 
            (new System.Text.RegularExpressions.Regex(@"^[\p{L}\p{N}\s.,!?'-]*$", RegexOptions.Compiled),
            new System.Text.RegularExpressions.Regex(@"^[[:alnum:]\s.,!?'-]*$", RegexOptions.Compiled), 
            250, 
            "Descriptions may include letters, numbers, spaces, and punctuation like .,!?"),
    };

    public enum Pattern
    {
        Title,
        Name,
        Address,
        Email,
        Password,
        PhoneE164,
        Domain,
        Description
    }
}