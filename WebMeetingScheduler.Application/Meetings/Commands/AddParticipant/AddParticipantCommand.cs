using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record AddParticipantCommand(
    Guid ParticipantId,
    Guid MeetingId) : IRequest;