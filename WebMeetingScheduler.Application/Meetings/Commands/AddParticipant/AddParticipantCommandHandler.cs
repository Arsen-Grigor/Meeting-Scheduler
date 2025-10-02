using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class AddParticipantCommandHandler(
    IMeetingsRepository meetingsRepository,
    IParticipantsRepository participantsRepository)
    : IRequestHandler<AddParticipantCommand>
{
    public async Task Handle(AddParticipantCommand request, CancellationToken cancellationToken)
    {
        await meetingsRepository.AddParticipantToMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await participantsRepository.AddMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
    }
}