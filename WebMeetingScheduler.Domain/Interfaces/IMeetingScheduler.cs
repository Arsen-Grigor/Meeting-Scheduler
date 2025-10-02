using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Domain.Interfaces;

public interface IMeetingScheduler
{
    DateTime? TryScheduleMeeting(
        List<Meeting> existingMeetings,
        List<ParticipantId> participantIds,
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd);
}