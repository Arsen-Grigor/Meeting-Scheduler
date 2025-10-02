using MediatR;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed class GetParticipantMeetingsQueryHandler : IRequestHandler<GetParticipantMeetingsQuery, List<MeetingDto>?>
{
    private readonly IParticipantsRepository _participantsRepository;
    private readonly IMeetingsRepository _meetingsRepository;

    public GetParticipantMeetingsQueryHandler(IParticipantsRepository repository, IMeetingsRepository meetingsRepository)
    {
        _participantsRepository = repository;
        _meetingsRepository = meetingsRepository;
    }
    
    public async Task<List<MeetingDto>?> Handle(GetParticipantMeetingsQuery request, CancellationToken cancellationToken)
    {
        var participant = await _participantsRepository.GetParticipantByIdAsync(request.ParticipantId, cancellationToken);
        List<MeetingDto>? meetings = new List<MeetingDto>();
        foreach (var m in participant.Meetings)
        {
            meetings.Add((await _meetingsRepository.GetMeetingAsync(m, cancellationToken)).MapMeetingToDto());
        }
        
        return meetings;
    }
}