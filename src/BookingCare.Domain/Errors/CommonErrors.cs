using BookingCare.Domain.Common;

public static class CommonErrors
{
    public static readonly Error Unauthorized =
        new("Common.Unauthorized", "You are not authorized to perform this action.");

    public static readonly Error Forbidden =
        new("Common.Forbidden", "You do not have permission to access this resource.");

    public static readonly Error NotFound =
        new("Common.NotFound", "The requested resource was not found.");

    public static readonly Error InvalidOperation =
        new("Common.InvalidOperation", "This operation is not allowed.");
}