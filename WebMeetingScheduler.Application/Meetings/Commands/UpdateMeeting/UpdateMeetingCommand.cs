using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record UpdateMeetingCommand(
    Guid MeetingId,
    string? Title,
    string? Description) : IRequest;