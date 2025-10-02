using MediatR;
using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed record GetParticipantQuery(
    Guid ParticipantId) : IRequest<ParticipantDto?>;