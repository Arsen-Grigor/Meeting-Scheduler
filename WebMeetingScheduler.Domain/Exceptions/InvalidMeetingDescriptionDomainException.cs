namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidMeetingDescriptionDomainException(string description)
    : Exception($"Description of '{description}' shouldn't consist of more than 500 characters.");