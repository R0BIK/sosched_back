using FluentValidation;
using SoschedBack.Core.Common.Regex;

namespace SoschedBack.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValidString<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x);
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValidTitle<T>(
        this IRuleBuilder<T, string> ruleBuilder) 
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("Title contains invalid characters.");
        // .ApplyRegexPattern(RegexPatterns.Pattern.Title);
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValidName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("Name contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Name);
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValidDescription<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("Description contains invalid characters.");
        // .ApplyRegexPattern(RegexPatterns.Pattern.Description);
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValidAddress<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("Address contains invalid characters.");
        // .ApplyRegexPattern(RegexPatterns.Pattern.Address);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidPhoneNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("PhoneE164 contains invalid characters.");
        // .ApplyRegexPattern(RegexPatterns.Pattern.PhoneE164);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .MustBeValidString()
            .WithMessage("Email contains invalid characters.");
        // .ApplyRegexPattern(RegexPatterns.Pattern.Email);
    }
    
    public static IRuleBuilderOptions<T, int> MustBeValidId<T>(
        this IRuleBuilder<T, int> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Id is required.")
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id must be greater or equal to 1");
    }
    
    public static IRuleBuilderOptions<T, IEnumerable<int>> MustBeValidListOfIds<T>(
        this IRuleBuilder<T, IEnumerable<int>> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Ids must be provided and contain at least one item.")
            .ForEach(id =>
                id.GreaterThanOrEqualTo(1)
                    .WithMessage("Ids must be greater or equal to 1.")
            );
    }
    
    public static IRuleBuilderOptions<T, string?> MustBeValidSortField<T, TEnum>(this IRuleBuilder<T, string?> ruleBuilder)
        where TEnum : Enum
    {
        var allowedSortFields = Enum
            .GetNames(typeof(TEnum))
            .Select(name => name.ToLower())
            .ToArray();

        return ruleBuilder
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || allowedSortFields.Contains(sortBy.ToLower()))
            .WithMessage($"SortBy must be one of the following: {string.Join(", ", allowedSortFields)}");
    }
    
    // public static IRuleBuilderOptions<T, IEnumerable<int>> MustBeValidEntityIdsList<T, TEntity>(
    //     this IRuleBuilder<T, IEnumerable<int>> ruleBuilder,
    //     SoschedBackDbContext db)
    //     where TEntity : class
    // {
    //     return ruleBuilder.MustAsync(async (ids, cancellationToken) =>
    //     {
    //         var count = await db.Set<TEntity>()
    //             .CountAsync(e => ids.Contains(e.Id), cancellationToken);
    //
    //         return count == ids.Distinct().Count();
    //     }).WithMessage($"{typeof(TEntity).Name}s Ids are invalid.");
    // }
    
    //TODO: Space configuration
    // public static IRuleBuilderOptions<T, IEnumerable<int>> MustBeValidInstitutionEntityIdsList<T, TEntity>(
    //     this IRuleBuilder<T, IEnumerable<int>> ruleBuilder,
    //     TimetileDbContext db,
    //     int institutionId)
    //     where TEntity : class, IInstitutionEntity
    // {
    //     return ruleBuilder.MustAsync(async (ids, cancellationToken) =>
    //     {
    //         var count = await db.Set<TEntity>()
    //             .Where(e => e.InstitutionId == institutionId)
    //             .CountAsync(e => ids.Contains(e.Id), cancellationToken);
    //
    //         return count == ids.Distinct().Count();
    //     }).WithMessage($"{typeof(TEntity).Name}s Ids are invalid.");
    // }
    
    public static IRuleBuilderOptions<T, string> ApplyRegexPattern<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        RegexPatterns.Pattern patternKey,
        bool allowEmpty = false) where T : class
    {
        var patternInfo = RegexPatterns.Patterns[patternKey];

        var options = allowEmpty
            ? ruleBuilder
                .Matches(patternInfo.Pattern).WithMessage(patternInfo.Description)
                .MaximumLength(patternInfo.MaxLength)
                .WithMessage($"{patternKey} cannot exceed {patternInfo.MaxLength} characters.")
            : ruleBuilder
                .NotEmpty().WithMessage($"{patternKey} is required.")
                .Matches(patternInfo.Pattern).WithMessage(patternInfo.Description)
                .MaximumLength(patternInfo.MaxLength)
                .WithMessage($"{patternKey} cannot exceed {patternInfo.MaxLength} characters.");

        return options;
    }
}