using MediatR;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Queries;

public sealed class GetMeetingQueryHandler : IRequestHandler<GetMeetingQuery, MeetingDto?>
{
    private readonly IMeetingsRepository _meetingsRepository;

    public GetMeetingQueryHandler(IMeetingsRepository repository)
    {
        _meetingsRepository = repository;
    }
    
    public async Task<MeetingDto?> Handle(GetMeetingQuery request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingsRepository.GetMeetingAsync(request.MeetingId, cancellationToken);
        if (meeting != null) return MappingExtensions.MapMeetingToDto(meeting);
        return null;
    }
}