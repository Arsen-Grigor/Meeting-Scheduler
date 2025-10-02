using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Meetings;

public sealed record MeetingTitle
{
    public string Value { get; init; }
    public MeetingTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) ||
            title.Length < MeetingConstants.Meeting.MeetingTitleMinLength ||
            title.Length > MeetingConstants.Meeting.MeetingTitleMaxLength)
        {
            throw new InvalidMeetingTitleDomainException(title);
        }
            
        Value = title;
    }
        
    public static implicit operator string(MeetingTitle title) => title?.Value;
    public static implicit operator MeetingTitle(string value) => value is null ? null :  new MeetingTitle(value);
    
}