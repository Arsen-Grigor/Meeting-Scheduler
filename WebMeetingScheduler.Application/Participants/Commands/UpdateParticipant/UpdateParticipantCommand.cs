using MediatR;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed record UpdateParticipantCommand(
    Guid ParticipantId,
    string? FullName,
    string? Role,
    string? Email) : IRequest;