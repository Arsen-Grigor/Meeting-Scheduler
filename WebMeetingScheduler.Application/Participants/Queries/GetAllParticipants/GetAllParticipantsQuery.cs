using MediatR;
using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Application.Participants.Queries;

public sealed record GetAllParticipantsQuery() : IRequest<List<ParticipantDto>?>;