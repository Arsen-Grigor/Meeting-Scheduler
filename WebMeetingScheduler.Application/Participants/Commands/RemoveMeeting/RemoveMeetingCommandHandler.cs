using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed class RemoveMeetingCommandHandler(
    IParticipantsRepository participantsRepository,
    IMeetingsRepository meetingsRepository)
    : IRequestHandler<RemoveMeetingCommand>
{
    public async Task Handle(RemoveMeetingCommand request, CancellationToken cancellationToken)
    {
        await participantsRepository.RemoveMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await meetingsRepository.RemoveParticipantFromMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await meetingsRepository.SaveChangesToDbContextAsync(cancellationToken);
        await participantsRepository.SaveChangesToDbContextAsync(cancellationToken);
    }
}