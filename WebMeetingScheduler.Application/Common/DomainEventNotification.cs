using MediatR;
using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Application.Common;

public class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}