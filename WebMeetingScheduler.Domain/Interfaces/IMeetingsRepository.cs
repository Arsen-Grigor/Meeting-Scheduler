using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Domain.Interfaces;

public interface IMeetingsRepository
{
    Task CreateMeetingAsync(
        Meeting meeting,
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd,
        CancellationToken cancellationToken = default);
    Task UpdateMeetingAsync(
        MeetingId meetingId,
        MeetingTitle? title = null,
        MeetingDescription? description = null,
        CancellationToken cancellationToken = default);
    Task DeleteMeetingAsync(
        MeetingId meeting,
        CancellationToken cancellationToken = default);
    Task AddParticipantToMeetingAsync(
        ParticipantId participantId,
        MeetingId meetingId,
        CancellationToken cancellationToken = default);
    Task RemoveParticipantFromMeetingAsync(
        ParticipantId participantId,
        MeetingId meetingId,
        CancellationToken cancellationToken = default);
    Task<Meeting?> GetMeetingAsync(
        MeetingId meetingId,
        CancellationToken cancellationToken = default);
    Task<List<Meeting>?> GetAllMeetingsAsync(
        CancellationToken cancellationToken = default);
}