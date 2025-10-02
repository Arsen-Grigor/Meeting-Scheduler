using MediatR;

namespace WebMeetingScheduler.Application.Meetings.Commands;

public sealed record CreateMeetingCommand(
    string Title,
    string Description,
    int Duration,
    DateTime earliestStart,
    DateTime latestEnd) : IRequest<Guid>;