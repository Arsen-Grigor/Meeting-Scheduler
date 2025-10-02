using MediatR;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed record RemoveMeetingCommand(
    Guid ParticipantId,
    Guid MeetingId) : IRequest;