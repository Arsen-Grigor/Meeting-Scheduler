namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidMeetingTimeRangeDomainException(DateTime startTime, DateTime endTime)
    : Exception($"Meeting end time '{endTime}' must be after start time '{startTime}'.");