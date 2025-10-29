namespace SoschedBack.Core.Common.UnifiedResponse;

public class Error
{
    public string Message { get; }
    public string Code { get; }
    public IReadOnlyList<Error>? Details { get; }
    
    public bool HasDetails => Details?.Any() == true;
    
    public Error(string message, string code = "", IReadOnlyList<Error>? details = null)
    {
        Message = message;
        Code = code;
        Details = details;
    }

    public static Error None => new(string.Empty);

    public static Error From(string message, string code = "") => new(message, code);

    public static Error Aggregate(string message, params Error[] errors)
        => new(message, code: "MULTIPLE_ERRORS", details: errors.ToList());
    
    public static implicit operator Error(string message) => new(message);

    public static implicit operator string(Error error) => error.Message;

    public override string ToString()
    {
        if (!HasDetails)
            return $"{Code}: {Message}".Trim(':', ' ');

        var detailedMessages = string.Join("; ", Details!.Select(d => d.ToString()));
        return $"{Code}: {Message} ({detailedMessages})".Trim(':', ' ');
    }
}