using MediatR;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, Guid>
{
    private readonly IMeetingsRepository _meetingsRepository;

    public CreateMeetingCommandHandler(IMeetingsRepository repository)
    {
        _meetingsRepository = repository;
    }
    
    public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var meetingId = Guid.NewGuid();
        var meeting = Meeting.Create(meetingId, request.Title, request.Description);
        await _meetingsRepository.CreateMeetingAsync(
            meeting,
            request.Duration,
            request.earliestStart,
            request.latestEnd,
            cancellationToken);
        
        return meetingId;
    }
}