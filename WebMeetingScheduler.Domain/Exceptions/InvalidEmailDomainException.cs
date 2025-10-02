namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidEmailDomainException(string email)
    : Exception($"The email address '{email}' is not valid.");
