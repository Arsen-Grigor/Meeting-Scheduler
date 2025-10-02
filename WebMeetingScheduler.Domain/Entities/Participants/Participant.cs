using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Domain.Entities.Participants;

public class Participant : Entity
{
    public ParticipantId Id { get; private set; }
    public FullName fullName { get; private set; }
    public Roles Role { get; private set; }
    public Email Email { get; private set; }
    private List<MeetingId> _meetings = new List<MeetingId>();
    public IReadOnlyCollection<MeetingId> Meetings => _meetings;

    public static Participant Create(
        Guid id,
        string fullName,
        string role,
        string email)
    {
        return new Participant(id, fullName, Roles.From(role), email);
    }
    
    private Participant(){ }

    public void UpdateData(FullName? newFullName, Roles? newRole, Email? newEmail)
    {
        if (newFullName is not null)
        {
            fullName = newFullName;
        }

        if (newRole is not null)
        {
            Role = newRole;
        }
        
        if (newEmail is not null)
        {
            Email = newEmail;
        }
    }

    public void AddMeeting(MeetingId meetingId)
    {
        if (_meetings.All(p => p != meetingId))
        {
            _meetings = _meetings.Concat(new[] { meetingId }).ToList();
        }
        
        AddDomainEvent(new MeetingParticipantAddedEvent(meetingId.Value, this.Id.Value));
    }

    public void RemoveMeeting(MeetingId meetingId)
    {
        var existing = _meetings.FirstOrDefault(p => p == meetingId);
        if (existing != null)
            _meetings = _meetings.Where(m => m != meetingId).ToList();
        
        AddDomainEvent(new MeetingParticipantRemovedEvent(meetingId.Value, this.Id.Value));
    }
    
    private Participant(ParticipantId id, FullName fullName, Roles role, Email email)
    {
        Id = id;
        this.fullName = fullName;
        Role = role;
        Email = email;
        
        AddDomainEvent(new ParticipantCreatedEvent(this.Id.Value));
    }
}