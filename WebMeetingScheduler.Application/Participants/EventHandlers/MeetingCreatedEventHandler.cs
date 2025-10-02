using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Participants.EventHandlers;

public sealed class ParticipantCreatedEventHandler : INotificationHandler<DomainEventNotification<ParticipantCreatedEvent>>
{
    private readonly ILogger<ParticipantCreatedEventHandler> _logger;

    public ParticipantCreatedEventHandler(ILogger<ParticipantCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<ParticipantCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}