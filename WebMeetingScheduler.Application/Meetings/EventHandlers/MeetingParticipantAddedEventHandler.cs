using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Meetings.EventHandlers;

public sealed class MeetingParticipantAddedEventHandler : INotificationHandler<DomainEventNotification<MeetingParticipantAddedEvent>>
{
    private readonly ILogger<MeetingParticipantAddedEventHandler> _logger;

    public MeetingParticipantAddedEventHandler(ILogger<MeetingParticipantAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<MeetingParticipantAddedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}