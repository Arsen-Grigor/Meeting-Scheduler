using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Domain.Events;

public sealed record MeetingParticipantAddedEvent(Guid MeetingId, Guid ParticipantId) : IDomainEvent;