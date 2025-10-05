using MediatR;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class CreateMeetingCommandHandler(
    IMeetingsRepository repository,
    IParticipantsRepository participantsRepository)
    : IRequestHandler<CreateMeetingCommand, Guid>
{
    public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var meetingId = Guid.NewGuid();
        var meeting = Meeting.Create(meetingId, request.Title, request.Description, request.ParticipantId);
        await repository.CreateMeetingAsync(
            meeting,
            request.Duration,
            request.EarliestStart,
            request.LatestEnd,
            cancellationToken);

        await Task.WhenAll();
        
        return meetingId;
    }
}