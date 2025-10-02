using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Events;
using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Meetings;

public class Meeting : Entity
{
    public MeetingId Id { get; private set; }
    public MeetingTitle Title { get; private set; }
    public MeetingDescription Description { get; private set; }
    private DateTime? _startTime;
    private DateTime? _endTime;
    private int? _durationMinutes;
    public DateTime? StartTime
    {
        get => _startTime;
        private set
        {
            _startTime = value;
            ValidateTimeEdges();
        }
    }
    public DateTime? EndTime
    {
        get => _endTime;
        private set
        {
            _endTime = value;
            ValidateTimeEdges();
        }
    }

    public int? Duration
    {
        get => _durationMinutes;
        private set
        {
            _durationMinutes = value;
            ValidateDuration();
        }
    }
    public IReadOnlyCollection<ParticipantId> ParticipantsId => _participantsId.AsReadOnly();
    private List<ParticipantId> _participantsId = new List<ParticipantId>();
    
    public static Meeting Create(
        Guid id,
        string title,
        string description)
    {
        return new Meeting(id, title, description);
    }
    
    private Meeting() { }
    
    private Meeting(MeetingId id, MeetingTitle title, MeetingDescription description)
    {
        Id = id;
        Title = title;
        Description = description;
        
        AddDomainEvent(new MeetingCreatedEvent(id.Value));
    }

    public void UpdateTextData(MeetingTitle? title = null, MeetingDescription? description = null)
    {
        if (title is not null)
        {
            Title = title;
        }

        if (description is not null)
        {
            Description = description;
        }
        
        AddDomainEvent(new MeetingUpdatedEvent(Id.Value));
    }
    
    public void AddParticipant(ParticipantId participantId)
    {
        if (_participantsId.All(p => p != participantId))
            _participantsId = _participantsId.Concat(new[] { participantId }).ToList();
        
        AddDomainEvent(new MeetingParticipantAddedEvent(this.Id.Value, participantId.Value));
    }

    public void RemoveParticipant(ParticipantId participantId)
    {
        var existing = _participantsId.FirstOrDefault(p => p == participantId);
        if (existing != null)
            _participantsId = _participantsId.Where(p => p != participantId).ToList();
        
        AddDomainEvent(new MeetingParticipantRemovedEvent(this.Id.Value, participantId.Value));
    }

    public void RemoveAllParticipants()
    {
        foreach (var p in _participantsId)
        {
            AddDomainEvent(new MeetingParticipantRemovedEvent(p.Value, this.Id.Value));
        }
    
        _participantsId = new List<ParticipantId>();
        
    }

    public void Schedule(DateTime start, DateTime end)
    {
        StartTime = start;
        EndTime = end;
    }
    
    private void ValidateTimeEdges()
    {
        if (!_startTime.HasValue || !_endTime.HasValue)
            return;

        if (_startTime.Value >= _endTime.Value)
        {
            throw new InvalidMeetingTimeRangeDomainException(_startTime.Value, _endTime.Value);
        }
    }

    private void ValidateDuration()
    {
        if (Duration.HasValue && (Duration < MeetingConstants.Meeting.MinimumDurationMinutes ||
            Duration > MeetingConstants.Meeting.MaximumDurationMinutes))
        {
            throw new InvalidMeetingDurationDomainException(
                Duration,
                MeetingConstants.Meeting.MinimumDurationMinutes,
                MeetingConstants.Meeting.MaximumDurationMinutes);
        }
    }
}