using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed class DeleteMeetingCommandHandler : IRequestHandler<DeleteMeetingCommand>
{
    private readonly IMeetingsRepository _meetingsRepository;

    public DeleteMeetingCommandHandler(IMeetingsRepository repository)
    {
        _meetingsRepository = repository;
    }
    
    public async Task Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
    {
        await _meetingsRepository.DeleteMeetingAsync(request.MeetingId, cancellationToken);
    }
}