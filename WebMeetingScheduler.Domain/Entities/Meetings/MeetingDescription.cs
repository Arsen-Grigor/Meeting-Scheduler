using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Meetings;

public sealed record MeetingDescription
{
    public string Value { get; init; }
    public MeetingDescription(string text)
    {
        if (text.Length > MeetingConstants.Meeting.MeetingDescriptionMaxLength)
        {
            throw new InvalidMeetingDescriptionDomainException(text);
        }
            
        Value = text;
    }
        
    public static implicit operator string(MeetingDescription text) => text?.Value;
    public static implicit operator MeetingDescription(string value) => value is null ? null : new MeetingDescription(value);
}