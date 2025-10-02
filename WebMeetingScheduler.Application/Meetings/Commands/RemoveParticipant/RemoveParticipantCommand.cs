using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record RemoveParticipantCommand(
    Guid ParticipantId,
    Guid MeetingId) : IRequest;