using MediatR;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed class GetAllParticipantsQueryHandler : IRequestHandler<GetAllParticipantsQuery, List<ParticipantDto>?>
{
    private readonly IParticipantsRepository _participantsRepository;

    public GetAllParticipantsQueryHandler(IParticipantsRepository repository)
    {
        _participantsRepository = repository;
    }
    
    public async Task<List<ParticipantDto>?> Handle(GetAllParticipantsQuery request, CancellationToken cancellationToken)
    {
        var participantsRepo = await _participantsRepository.GetAllParticipantsAsync(cancellationToken);
        List<ParticipantDto> participantsDto = new List<ParticipantDto>();
        if (participantsRepo != null)
        {
            foreach (var p in participantsRepo)
            {
                participantsDto.Add(p.MapParticipantToDto());
            }
            
            return participantsDto;
        }

        return null;
    }
}