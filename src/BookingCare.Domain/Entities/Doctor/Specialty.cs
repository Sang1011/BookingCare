using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;

namespace BookingCare.Domain.Entities.Doctor;

public class Specialty : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Specialty() { }

    public static Result<Specialty> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Specialty>.Failure(SpecialtyErrors.NameRequired);

        if (name.Length > 200)
            return Result<Specialty>.Failure(SpecialtyErrors.NameTooLong);

        var specialty = new Specialty
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            IsActive = true
        };

        specialty.Touch();
        return Result<Specialty>.Success(specialty);
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(SpecialtyErrors.NameRequired);

        if (name.Length > 200)
            return Result.Failure(SpecialtyErrors.NameTooLong);

        Name = name.Trim();
        Description = description?.Trim();
        Touch();
        return Result.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}