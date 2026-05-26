using BookingCare.Domain.Common;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Errors;

namespace BookingCare.Domain.Entities.Doctor;

public class Doctor : AuditableEntity
{
    public Guid SpecialtyId { get; private set; }
    public string LicenseNumber { get; private set; } = string.Empty;
    public int YearsOfExperience { get; private set; }
    public string? Bio { get; private set; }
    public decimal ConsultationFee { get; private set; }
    public string? AvatarUrl { get; private set; }
    public Specialty Specialty { get; private set; } = null!;
    public User User { get; private set; } = null!;
    public bool IsVerified { get; private set; }
    public IReadOnlyCollection<DoctorSchedule> Schedules => _schedules.AsReadOnly();
    private readonly List<DoctorSchedule> _schedules = [];

    private Doctor() { }

    public static Result<Doctor> Create(
        Guid userId,
        Guid specialtyId,
        string licenseNumber,
        int yearsOfExperience,
        decimal consultationFee,
        string? bio = null,
        string? avatarUrl = null)
    {
        if (userId == Guid.Empty)
            return Result<Doctor>.Failure(DoctorErrors.InvalidUserId);

        if (string.IsNullOrWhiteSpace(licenseNumber))
            return Result<Doctor>.Failure(DoctorErrors.LicenseNumberRequired);

        if (yearsOfExperience < 0)
            return Result<Doctor>.Failure(DoctorErrors.InvalidYearsOfExperience);

        if (consultationFee < 0)
            return Result<Doctor>.Failure(DoctorErrors.InvalidConsultationFee);

        var doctor = new Doctor
        {
            SpecialtyId = specialtyId,
            LicenseNumber = licenseNumber.Trim(),
            YearsOfExperience = yearsOfExperience,
            ConsultationFee = consultationFee,
            Bio = bio?.Trim(),
            AvatarUrl = avatarUrl?.Trim(),
            IsVerified = false
        };

        doctor.Touch();
        return Result<Doctor>.Success(doctor);
    }

    public Result UpdateProfile(
        Guid specialtyId,
        int yearsOfExperience,
        decimal consultationFee,
        string? bio,
        string? avatarUrl)
    {
        if (yearsOfExperience < 0)
            return Result.Failure(DoctorErrors.InvalidYearsOfExperience);

        if (consultationFee < 0)
            return Result.Failure(DoctorErrors.InvalidConsultationFee);

        SpecialtyId = specialtyId;
        YearsOfExperience = yearsOfExperience;
        ConsultationFee = consultationFee;
        Bio = bio?.Trim();
        AvatarUrl = avatarUrl?.Trim();
        Touch();
        return Result.Success();
    }

    public void Verify()
    {
        IsVerified = true;
        Touch();
    }

    public void Unverify()
    {
        IsVerified = false;
        Touch();
    }
}