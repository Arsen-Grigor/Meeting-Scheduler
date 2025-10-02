using MediatR;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Meetings.Queries;

public sealed class GetAllMeetingsQueryHandler : IRequestHandler<GetAllMeetingsQuery, List<MeetingDto>?>
{
    private readonly IMeetingsRepository _meetingsRepository;

    public GetAllMeetingsQueryHandler(IMeetingsRepository repository)
    {
        _meetingsRepository = repository;
    }

    public async Task<List<MeetingDto>?> Handle(GetAllMeetingsQuery request, CancellationToken cancellationToken)
    {
        var meetingsRepo = await _meetingsRepository.GetAllMeetingsAsync(cancellationToken);
        List<MeetingDto> meetingsDto = new List<MeetingDto>();
        if (meetingsRepo != null)
        {
            foreach (var m in meetingsRepo)
            {
                meetingsDto.Add(m.MapMeetingToDto());
            }
            
            return meetingsDto;
        }

        return null;
    }
}