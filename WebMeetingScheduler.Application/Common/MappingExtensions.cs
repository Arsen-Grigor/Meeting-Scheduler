using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Application.Common;

public static class MappingExtensions
{
    public static MeetingDto MapMeetingToDto(this Meeting meeting)
    {
        List<Guid> temp = new List<Guid>();
        
        foreach (var p in meeting.ParticipantsId)
        {
            temp.Add(p.Value);
        }
        
        return new MeetingDto(
            meeting.Id.Value,
            meeting.Title.Value,
            meeting.Description.Value,
            meeting.StartTime,
            meeting.EndTime,
            temp);
    }
    
    public static ParticipantDto MapParticipantToDto(this Participant participant)
    {
        List<Guid> temp = new List<Guid>();
        
        foreach (var m in participant.Meetings)
        {
            temp.Add(m.Value);
        }

        return new ParticipantDto(
            participant.Id.Value,
            participant.fullName.Value,
            participant.Role.Code,
            participant.Email.Value,
            temp);
    }
}