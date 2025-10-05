using MediatR;
using WebMeetingScheduler.Application.Meetings.Commands;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Participants.Commands;

public sealed class AddMeetingCommandHandler : IRequestHandler<AddMeetingCommand>
{
    private readonly IParticipantsRepository _participantsRepository;
    private readonly IMeetingsRepository _meetingsRepository;

    public AddMeetingCommandHandler(
        IParticipantsRepository participantsRepository,
        IMeetingsRepository meetingsRepository)
    {
        _participantsRepository = participantsRepository;
        _meetingsRepository = meetingsRepository;
    }
    
    public async Task Handle(AddMeetingCommand request, CancellationToken cancellationToken)
    {
        await _participantsRepository.AddMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await _meetingsRepository.AddParticipantToMeetingAsync(
            request.ParticipantId,
            request.MeetingId,
            cancellationToken);
        await _meetingsRepository.SaveChangesToDbContextAsync(cancellationToken);
        await _participantsRepository.SaveChangesToDbContextAsync(cancellationToken);
    }
}