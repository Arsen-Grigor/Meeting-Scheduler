namespace WebMeetingScheduler.Application.Common;

public sealed record ParticipantDto(
    Guid ParticipantId,
    string FullName,
    string Role,
    string Email,
    List<Guid> Meetings
);