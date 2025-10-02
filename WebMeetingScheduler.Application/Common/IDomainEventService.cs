using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Common;

public interface IDomainEventService
{
    Task Publish(IDomainEvent domainEvent);
}