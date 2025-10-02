using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;

    public DomainEventService(
        ILogger<DomainEventService> logger, 
        IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Publish(IDomainEvent domainEvent)
    {
        _logger.LogInformation(
            "Publishing domain event: {DomainEvent}", 
            domainEvent.GetType().Name);

        var notification = CreateDomainEventNotification(domainEvent);
        await _mediator.Publish(notification);
    }

    private INotification CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>)
            .MakeGenericType(domainEvent.GetType());
        
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}