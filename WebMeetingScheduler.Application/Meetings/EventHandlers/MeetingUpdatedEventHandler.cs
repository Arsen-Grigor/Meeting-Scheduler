using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Meetings.EventHandlers;

public sealed class MeetingUpdatedEventHandler : INotificationHandler<DomainEventNotification<MeetingUpdatedEvent>>
{
    private readonly ILogger<MeetingUpdatedEventHandler> _logger;

    public MeetingUpdatedEventHandler(ILogger<MeetingUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<MeetingUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}