using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Domain.Events;

public sealed record MeetingUpdatedEvent(Guid Id) : IDomainEvent;