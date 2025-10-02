using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Meetings.EventHandlers;

public sealed class MeetingDeletedEventHandler : INotificationHandler<DomainEventNotification<MeetingDeletedEvent>>
{
    private readonly ILogger<MeetingDeletedEventHandler> _logger;

    public MeetingDeletedEventHandler(ILogger<MeetingDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<MeetingDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}