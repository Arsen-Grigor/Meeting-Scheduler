using MediatR;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed class GetParticipantQueryHandler : IRequestHandler<GetParticipantQuery, ParticipantDto?>
{
    private readonly IParticipantsRepository _participantsRepository;

    public GetParticipantQueryHandler(IParticipantsRepository repository)
    {
        _participantsRepository = repository;
    }
    
    public async Task<ParticipantDto?> Handle(GetParticipantQuery request, CancellationToken cancellationToken)
    {
        var participant = await _participantsRepository.GetParticipantByIdAsync(request.ParticipantId, cancellationToken);
        if (participant != null) return MappingExtensions.MapParticipantToDto(participant);
        return null;
    }
}