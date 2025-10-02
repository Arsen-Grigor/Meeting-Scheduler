using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Domain.Events;

public sealed record MeetingCreatedEvent(Guid Id) : IDomainEvent;