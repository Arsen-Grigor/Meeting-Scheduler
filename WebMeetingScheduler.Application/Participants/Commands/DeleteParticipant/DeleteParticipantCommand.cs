using MediatR;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed record DeleteParticipantCommand(
    Guid ParticipantId) : IRequest;