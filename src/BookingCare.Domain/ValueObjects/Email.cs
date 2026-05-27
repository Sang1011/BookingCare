using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;

namespace BookingCare.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Result<Email>.Failure(UserErrors.EmailRequired);

        var normalized = raw.Trim().ToLowerInvariant();

        if (!normalized.Contains('@') || normalized.Length > 254)
            return Result<Email>.Failure(UserErrors.InvalidEmailFormat);

        return Result<Email>.Success(new Email(normalized));
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}