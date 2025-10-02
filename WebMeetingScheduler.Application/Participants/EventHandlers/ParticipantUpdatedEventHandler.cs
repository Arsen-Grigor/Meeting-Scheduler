using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Participants.EventHandlers;

public sealed class ParticipantUpdatedEventHandler : INotificationHandler<DomainEventNotification<ParticipantUpdatedEvent>>
{
    private readonly ILogger<ParticipantUpdatedEventHandler> _logger;

    public ParticipantUpdatedEventHandler(ILogger<ParticipantUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<ParticipantUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}