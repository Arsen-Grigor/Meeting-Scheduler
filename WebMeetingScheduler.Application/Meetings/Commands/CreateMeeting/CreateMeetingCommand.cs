using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record CreateMeetingCommand(
    string Title,
    string Description,
    int Duration,
    DateTime EarliestStart,
    DateTime LatestEnd,
    List<Guid> ParticipantsId) : IRequest<Guid>;