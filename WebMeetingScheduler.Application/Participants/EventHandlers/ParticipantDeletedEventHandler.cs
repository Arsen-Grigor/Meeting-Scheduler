using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Participants.EventHandlers;

public sealed class ParticipantDeletedEventHandler : INotificationHandler<DomainEventNotification<ParticipantDeletedEvent>>
{
    private readonly ILogger<ParticipantDeletedEventHandler> _logger;

    public ParticipantDeletedEventHandler(ILogger<ParticipantDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<ParticipantDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}