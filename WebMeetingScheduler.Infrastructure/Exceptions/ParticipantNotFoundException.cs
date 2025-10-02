namespace WebMeetingScheduler.Infrastructure.Exceptions;

public class ParticipantNotFoundException(Guid participantId)
    : InfrastructureException($"Participant with ID '{participantId}' was not found.");