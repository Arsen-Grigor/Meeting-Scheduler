namespace WebMeetingScheduler.Application.Common;

public sealed record 
    MeetingDto(
    Guid MeetingId,
    string Title,
    string Description,
    DateTime? Start,
    DateTime? End,
    List<Guid> Participants
    );
