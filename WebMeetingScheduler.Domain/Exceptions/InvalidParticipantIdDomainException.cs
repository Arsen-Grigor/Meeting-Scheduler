namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidParticipantIdDomainException(Guid id)
    : Exception($"The provided GUID '{id}' is not a valid Participant ID.");