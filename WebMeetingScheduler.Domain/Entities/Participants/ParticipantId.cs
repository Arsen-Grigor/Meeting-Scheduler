using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Participants;

public sealed record ParticipantId(Guid Value)
{
    public static implicit operator Guid(ParticipantId id) => id.Value;

    public static implicit operator ParticipantId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new InvalidParticipantIdDomainException(id);
        }
        
        return new ParticipantId(id);
    }
}
