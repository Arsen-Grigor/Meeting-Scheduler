using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Domain.Events;

public sealed record ParticipantDeletedEvent(Guid Id) : IDomainEvent;
