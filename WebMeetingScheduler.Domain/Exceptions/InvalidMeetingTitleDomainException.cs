namespace WebMeetingScheduler.Domain.Exceptions;

public class InvalidMeetingTitleDomainException(string title)
    : Exception($"Title of '{title}' should consist of more than 2 and less than 150 characters.");