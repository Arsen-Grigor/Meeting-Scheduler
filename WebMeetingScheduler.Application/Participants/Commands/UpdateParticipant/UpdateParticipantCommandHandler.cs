using MediatR;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed class UpdateParticipantCommandHandler : IRequestHandler<UpdateParticipantCommand>
{
    private readonly IParticipantsRepository _participantsRepository;

    public UpdateParticipantCommandHandler(IParticipantsRepository repository)
    {
        _participantsRepository = repository;
    }
    
    public async Task Handle(UpdateParticipantCommand request, CancellationToken cancellationToken)
    {
        await _participantsRepository.UpdateParticipantAsync(
            request.ParticipantId,
            request.FullName,
            request.Role,
            request.Email,
            cancellationToken);
    }
}