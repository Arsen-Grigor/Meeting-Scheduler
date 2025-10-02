using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record DeleteMeetingCommand(
    Guid MeetingId) : IRequest;