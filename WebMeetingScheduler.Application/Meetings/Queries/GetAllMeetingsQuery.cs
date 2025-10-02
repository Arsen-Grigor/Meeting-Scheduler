using MediatR;
using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Application.Meetings.Queries;

public sealed record GetAllMeetingsQuery() : IRequest<List<MeetingDto>?>;