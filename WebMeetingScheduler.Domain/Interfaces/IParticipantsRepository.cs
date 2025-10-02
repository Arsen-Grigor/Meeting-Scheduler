using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Domain.Interfaces;

public interface IParticipantsRepository
{
    Task CreateParticipantAsync(
        Participant participant,
        CancellationToken cancellationToken = default);
    Task AddMeetingAsync(
        ParticipantId participantId, 
        MeetingId meetingId,
        CancellationToken cancellationToken = default);
    Task RemoveMeetingAsync(
        ParticipantId participantId, 
        MeetingId meetingId,
        CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(
        ParticipantId participantId,
        FullName? fullName = null,
        Roles? role = null,
        Email? email = null,
        CancellationToken cancellationToken = default);
    Task DeleteParticipantAsync(
        ParticipantId participantId,
        CancellationToken cancellationToken = default);
    Task<Participant?> GetParticipantByIdAsync(
        ParticipantId participantId,
        CancellationToken cancellationToken = default);
    Task<List<Participant>?> GetAllParticipantsAsync(
        CancellationToken cancellationToken = default);
    Task<List<Meeting>?> GetParticipantMeetingsAsync(
        ParticipantId participantId,
        CancellationToken cancellationToken = default);
}