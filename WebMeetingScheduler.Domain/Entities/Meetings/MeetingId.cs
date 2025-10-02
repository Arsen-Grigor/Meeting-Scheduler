using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Meetings;

public sealed record MeetingId(Guid Value)
{
    public static implicit operator Guid(MeetingId id) => id.Value;

    public static implicit operator MeetingId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new InvalidMeetingIdDomainException(id);
        }
        
        return new MeetingId(id);
    }
}
