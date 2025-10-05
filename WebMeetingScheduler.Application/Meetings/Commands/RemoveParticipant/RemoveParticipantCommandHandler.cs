using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class RemoveParticipantCommandHandler(
    IMeetingsRepository meetingsRepository,
    IParticipantsRepository participantsRepository)
    : IRequestHandler<RemoveParticipantCommand>
{
    public async Task Handle(RemoveParticipantCommand request, CancellationToken cancellationToken)
    {
        await meetingsRepository.RemoveParticipantFromMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await participantsRepository.RemoveMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await meetingsRepository.SaveChangesToDbContextAsync(cancellationToken);
        await participantsRepository.SaveChangesToDbContextAsync(cancellationToken);

    }
}