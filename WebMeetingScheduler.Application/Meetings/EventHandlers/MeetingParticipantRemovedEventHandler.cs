using MediatR;
using Microsoft.Extensions.Logging;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Events;

namespace WebMeetingScheduler.Application.Meetings.EventHandlers;

public sealed class MeetingParticipantRemovedEventHandler : INotificationHandler<DomainEventNotification<MeetingParticipantRemovedEvent>>
{
    private readonly ILogger<MeetingParticipantRemovedEventHandler> _logger;

    public MeetingParticipantRemovedEventHandler(ILogger<MeetingParticipantRemovedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<MeetingParticipantRemovedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}