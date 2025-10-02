using MediatR;
using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed record GetParticipantMeetingsQuery(
    Guid ParticipantId) : IRequest<List<MeetingDto>?>;