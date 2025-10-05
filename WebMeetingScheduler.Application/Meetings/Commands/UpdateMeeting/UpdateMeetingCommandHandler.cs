using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class UpdateMeetingCommandHandler : IRequestHandler<UpdateMeetingCommand>
{
    private readonly IMeetingsRepository _meetingsRepository;

    public UpdateMeetingCommandHandler(IMeetingsRepository repository)
    {
        _meetingsRepository = repository;
    }
    
    public async Task Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
    {
        await _meetingsRepository.UpdateMeetingAsync(
            request.MeetingId,
            request.Title,
            request.Description,
            cancellationToken);
        await _meetingsRepository.SaveChangesToDbContextAsync(cancellationToken);
    }
}