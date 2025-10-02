using MediatR;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed record AddMeetingCommand(
    Guid ParticipantId,
    Guid MeetingId) : IRequest;