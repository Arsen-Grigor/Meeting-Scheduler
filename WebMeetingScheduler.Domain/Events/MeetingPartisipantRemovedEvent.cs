using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Domain.Events;

public sealed record MeetingParticipantRemovedEvent(Guid MeetingId, Guid ParticipantId) : IDomainEvent;