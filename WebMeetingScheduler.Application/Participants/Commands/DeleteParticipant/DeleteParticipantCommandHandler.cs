using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed class DeleteParticipantCommandHandler : IRequestHandler<DeleteParticipantCommand>
{
    private readonly IParticipantsRepository _participantsRepository;

    public DeleteParticipantCommandHandler(IParticipantsRepository repository)
    {
        _participantsRepository = repository;
    }
    
    public async Task Handle(DeleteParticipantCommand request, CancellationToken cancellationToken)
    {
        await _participantsRepository.DeleteParticipantAsync(request.ParticipantId, cancellationToken);
    }
}