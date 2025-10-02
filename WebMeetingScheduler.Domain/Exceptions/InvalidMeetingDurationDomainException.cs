namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidMeetingDurationDomainException(int? duration, int minMinutes, int maxMinutes)
    : Exception($"Meeting duration of {duration} minutes must be" +
                $"between {minMinutes} and {maxMinutes} minutes.");