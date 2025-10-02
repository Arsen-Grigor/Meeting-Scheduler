using MediatR;
using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Application.Meetings.Queries;

public sealed record GetMeetingQuery(
    Guid MeetingId) : IRequest<MeetingDto?>;