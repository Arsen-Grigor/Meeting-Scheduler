using MediatR;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed record CreateParticipantCommand(
    string FullName,
    string Role,
    string Email) : IRequest<Guid>;