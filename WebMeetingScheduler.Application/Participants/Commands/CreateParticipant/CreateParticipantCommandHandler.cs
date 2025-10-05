using MediatR;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed class CreateParticipantCommandHandler : IRequestHandler<CreateParticipantCommand, Guid>
{
    private readonly IParticipantsRepository _participantsRepository;

    public CreateParticipantCommandHandler(IParticipantsRepository repository)
    {
        _participantsRepository = repository;
    }
    
    public async Task<Guid> Handle(CreateParticipantCommand request, CancellationToken cancellationToken)
    {
        var participantId = Guid.NewGuid();
        var participant = Participant.Create(
            participantId,
            request.FullName,
            request.Role,
            request.Email);
        await _participantsRepository.CreateParticipantAsync(participant, cancellationToken);
        await _participantsRepository.SaveChangesToDbContextAsync(cancellationToken);
        return participantId;
    }
}