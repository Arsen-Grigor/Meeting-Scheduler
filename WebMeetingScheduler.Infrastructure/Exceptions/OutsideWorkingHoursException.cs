namespace WebMeetingScheduler.Infrastructure.Exceptions;

public class OutsideWorkingHoursException(DateTime startTime, DateTime endTime) : InfrastructureException(
    $"Meeting scheduled from {startTime:HH:mm} to {endTime:HH:mm} is outside working hours " +
    $"(Working hours are configurable via MeetingConstants).");