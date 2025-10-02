namespace WebMeetingScheduler.Infrastructure.Exceptions;

public class MeetingNotFoundException(Guid meetingId)
    : InfrastructureException($"Meeting with ID '{meetingId}' was not found.");