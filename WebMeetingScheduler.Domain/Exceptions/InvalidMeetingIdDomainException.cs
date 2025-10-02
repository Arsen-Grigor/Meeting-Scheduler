namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidMeetingIdDomainException(Guid id)
    : Exception($"The provided GUID '{id}' is not a valid Meeting ID.");