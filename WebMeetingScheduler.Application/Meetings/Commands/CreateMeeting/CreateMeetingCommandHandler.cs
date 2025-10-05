using MediatR;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class CreateMeetingCommandHandler(
    IMeetingsRepository meetingsRepository,
    IParticipantsRepository participantsRepository)
    : IRequestHandler<CreateMeetingCommand, Guid>
{
    public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var meetingId = Guid.NewGuid();
        var meeting = Meeting.Create(Guid.NewGuid(), request.Title, request.Description, request.ParticipantsId);
        await meetingsRepository.CreateMeetingAsync(
            meeting,
            request.Duration,
            request.EarliestStart,
            request.LatestEnd,
            cancellationToken);
        
        foreach (var p in request.ParticipantsId)
        {
            await participantsRepository.AddMeetingAsync(p, meetingId, cancellationToken);
        }
        
        await meetingsRepository.SaveChangesToDbContextAsync(cancellationToken);
        await participantsRepository.SaveChangesToDbContextAsync(cancellationToken);
        return meetingId;
    }
}