using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Meetings.EventHandlers;

public sealed class MeetingCreatedEventHandler : INotificationHandler<DomainEventNotification<MeetingCreatedEvent>>
{
    private readonly ILogger<MeetingCreatedEventHandler> _logger;

    public MeetingCreatedEventHandler(ILogger<MeetingCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<MeetingCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}