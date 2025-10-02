namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidFullNameDomainException(string fullName)
    : Exception($"Fullname of '{fullName}' should consist of more than 2 and less than 150 characters.");