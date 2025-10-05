using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;
using WebMeetingScheduler.Infrastructure.Exceptions;

namespace WebMeetingScheduler.Infrastructure.Services;

public class MeetingSchedulerService : IMeetingScheduler
{
    public DateTime? TryScheduleMeeting(
        List<Meeting> existingMeetings,
        List<ParticipantId> participantIds,
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd)
    {
        ValidateSchedulingParameters(durationMinutes, earliestStart, latestEnd);
        var conflictingMeetings = GetConflictingMeetings(
            existingMeetings,
            earliestStart,
            participantIds);
        var startTime = FindAvailableSlot(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            latestEnd,
            overlapMinutes: 0);

        if (startTime.HasValue)
            return startTime;

        startTime = FindAvailableSlot(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            latestEnd,
            overlapMinutes: MeetingConstants.Meeting.OverlapToleranceMinutes);

        if (startTime.HasValue)
            return startTime;

        var suggestions = GenerateSuggestions(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            latestEnd);

        throw new NoAvailableTimeSlotException(
            durationMinutes,
            earliestStart,
            latestEnd,
            suggestions);
    }

    private void ValidateSchedulingParameters(
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd)
    {
        if (durationMinutes < MeetingConstants.Meeting.MinimumDurationMinutes)
        {
            throw new InvalidSchedulingParametersException(
                $"Meeting duration ({durationMinutes} minutes) is less than the minimum allowed " +
                $"duration ({MeetingConstants.Meeting.MinimumDurationMinutes} minutes).");
        }

        if (durationMinutes > MeetingConstants.Meeting.MaximumDurationMinutes)
        {
            throw new InvalidSchedulingParametersException(
                $"Meeting duration ({durationMinutes} minutes) exceeds the maximum allowed " +
                $"duration ({MeetingConstants.Meeting.MaximumDurationMinutes} minutes).");
        }

        if (earliestStart >= latestEnd)
        {
            throw new InvalidSchedulingParametersException(
                $"Earliest start time ({earliestStart}) must be before latest end time ({latestEnd}).");
        }

        if ((latestEnd - earliestStart).TotalMinutes < durationMinutes)
        {
            throw new InvalidSchedulingParametersException(
                $"Time window ({(latestEnd - earliestStart).TotalMinutes} minutes) is smaller " +
                $"than the requested duration ({durationMinutes} minutes).");
        }

        if (earliestStart < DateTime.UtcNow)
        {
            throw new InvalidSchedulingParametersException(
                "Cannot schedule meetings in the past.");
        }
    }

    private List<Meeting> GetConflictingMeetings(
        List<Meeting> allMeetings,
        DateTime earliestStart,
        List<ParticipantId> participantIds)
    {
        return allMeetings
            .Where(m => m.StartTime != default && m.EndTime != default)
            .Where(m => m.StartTime.Value.Date == earliestStart.Date)
            .Where(m => m.ParticipantsId.Any(p => participantIds.Contains(p)))
            .OrderBy(m => m.StartTime)
            .ToList();
    }

    private DateTime? FindAvailableSlot(
        List<Meeting> conflictingMeetings,
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd,
        int overlapMinutes)
    {
        if (!conflictingMeetings.Any())
        {
            if (IsWithinWorkingHours(earliestStart, durationMinutes))
                return earliestStart;
            
            var workStart = earliestStart.Date.AddHours(MeetingConstants.Meeting.WorkingHoursStart);
            if (IsWithinWorkingHours(workStart, durationMinutes) && 
                workStart.AddMinutes(durationMinutes) <= latestEnd)
                return workStart;
            
            return null;
        }
        
        var beforeFirst = TryScheduleBeforeFirstMeeting(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            overlapMinutes);

        if (beforeFirst.HasValue && IsWithinWorkingHours(beforeFirst.Value, durationMinutes))
            return beforeFirst;

        var betweenMeetings = TryScheduleBetweenMeetings(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            latestEnd,
            overlapMinutes);

        if (betweenMeetings.HasValue && IsWithinWorkingHours(betweenMeetings.Value, durationMinutes))
            return betweenMeetings;

        var afterLast = TryScheduleAfterLastMeeting(
            conflictingMeetings,
            durationMinutes,
            earliestStart,
            latestEnd,
            overlapMinutes);

        if (afterLast.HasValue && IsWithinWorkingHours(afterLast.Value, durationMinutes))
            return afterLast;

        return null;
    }

    private DateTime? TryScheduleBeforeFirstMeeting(
        List<Meeting> conflictingMeetings,
        int durationMinutes,
        DateTime earliestStart,
        int overlapMinutes)
    {
        if (!conflictingMeetings.Any())
            return earliestStart;

        var firstMeeting = conflictingMeetings.First();
        if (!firstMeeting.StartTime.HasValue)
            return null;
        
        var availableMinutes = (firstMeeting.StartTime - earliestStart)?.TotalMinutes + overlapMinutes;
        if (availableMinutes >= durationMinutes)
        {
            return earliestStart;
        }
        
        return null;
    }

private DateTime? TryScheduleBetweenMeetings(
    List<Meeting> conflictingMeetings,
    int durationMinutes,
    DateTime earliestStart,
    DateTime latestEnd,
    int overlapMinutes)
{
    for (int i = 0; i < conflictingMeetings.Count - 1; i++)
    {
        var currentMeeting = conflictingMeetings[i];
        var nextMeeting = conflictingMeetings[i + 1];
        
        if (!currentMeeting.EndTime.HasValue || !nextMeeting.StartTime.HasValue)
            continue;

        var slot = TrySlotWithNoOverlap(
            currentMeeting.EndTime.Value,
            nextMeeting.StartTime.Value,
            durationMinutes,
            earliestStart,
            latestEnd);
        
        if (slot.HasValue)
            return slot;

        if (overlapMinutes > 0)
        {
            slot = TrySlotWithPreviousOverlap(
                currentMeeting.EndTime.Value,
                nextMeeting.StartTime.Value,
                durationMinutes,
                earliestStart,
                latestEnd,
                overlapMinutes);
            
            if (slot.HasValue)
                return slot;
        }

        if (overlapMinutes > 0)
        {
            slot = TrySlotWithNextOverlap(
                currentMeeting.EndTime.Value,
                nextMeeting.StartTime.Value,
                durationMinutes,
                earliestStart,
                latestEnd,
                overlapMinutes);
            
            if (slot.HasValue)
                return slot;
        }
    }

    return null;
}

private DateTime? TrySlotWithNoOverlap(
    DateTime previousEnd,
    DateTime nextStart,
    int durationMinutes,
    DateTime earliestStart,
    DateTime latestEnd)
{
    var potentialStart = previousEnd;
    
    if (potentialStart < earliestStart)
        potentialStart = earliestStart;
    
    var potentialEnd = potentialStart.AddMinutes(durationMinutes);
    
    if (potentialStart >= previousEnd && 
        potentialEnd <= nextStart && 
        potentialEnd <= latestEnd)
    {
        return potentialStart;
    }
    
    return null;
}

private DateTime? TrySlotWithPreviousOverlap(
    DateTime previousEnd,
    DateTime nextStart,
    int durationMinutes,
    DateTime earliestStart,
    DateTime latestEnd,
    int overlapMinutes)
{
    var potentialStart = previousEnd.AddMinutes(-overlapMinutes);
    
    if (potentialStart < earliestStart)
        potentialStart = earliestStart;
    
    var potentialEnd = potentialStart.AddMinutes(durationMinutes);
    
    if (potentialEnd <= nextStart && potentialEnd <= latestEnd)
    {
        return potentialStart;
    }
    
    return null;
}

private DateTime? TrySlotWithNextOverlap(
    DateTime previousEnd,
    DateTime nextStart,
    int durationMinutes,
    DateTime earliestStart,
    DateTime latestEnd,
    int overlapMinutes)
{
    var potentialEnd = nextStart.AddMinutes(overlapMinutes);
    var potentialStart = potentialEnd.AddMinutes(-durationMinutes);
    
    if (potentialStart < earliestStart)
        return null;
    
    if (potentialStart >= previousEnd && potentialEnd <= latestEnd)
    {
        return potentialStart;
    }
    
    return null;
}

    private DateTime? TryScheduleAfterLastMeeting(
        List<Meeting> conflictingMeetings,
        int durationMinutes,
        DateTime earliestStart,
        DateTime latestEnd,
        int overlapMinutes)
    {
        if (!conflictingMeetings.Any())
            return null;

        var lastMeeting = conflictingMeetings.Last();
        if (!lastMeeting.EndTime.HasValue)
            return null;

        var potentialStart = lastMeeting.EndTime.Value.AddMinutes(-overlapMinutes);
        if (potentialStart < earliestStart)
            potentialStart = earliestStart;
        
        var potentialEnd = potentialStart.AddMinutes(durationMinutes);
        if (potentialEnd <= latestEnd)
        {
            return potentialStart;
        }

        return null;
    }

    private bool IsWithinWorkingHours(DateTime startTime, int durationMinutes)
    {
        var endTime = startTime.AddMinutes(durationMinutes);
        
        var workStart = startTime.Date.AddHours(MeetingConstants.Meeting.WorkingHoursStart);
        var workEnd = startTime.Date.AddHours(MeetingConstants.Meeting.WorkingHoursEnd);

        return startTime >= workStart && endTime <= workEnd;
    }

    private List<string> GenerateSuggestions(
        List<Meeting> conflictingMeetings,
        int requestedDuration,
        DateTime earliestStart,
        DateTime latestEnd)
    {
        var suggestions = new List<string>();

        var largestGap = FindLargestGap(conflictingMeetings, earliestStart, latestEnd);
        if (largestGap > 0 && largestGap < requestedDuration)
        {
            suggestions.Add($"Try reducing meeting duration to {largestGap} minutes or less.");
        }

        var extendedEnd = latestEnd.AddDays(1);
        var futureSlot = FindAvailableSlot(
            conflictingMeetings,
            requestedDuration,
            latestEnd,
            extendedEnd,
            MeetingConstants.Meeting.OverlapToleranceMinutes);

        if (futureSlot.HasValue)
        {
            suggestions.Add($"Extend search window - a slot is available on {futureSlot.Value:yyyy-MM-dd HH:mm}.");
        }

        if (conflictingMeetings.Count > 1)
        {
            suggestions.Add("Consider reducing the number of participants or splitting into multiple meetings.");
        }

        if (!suggestions.Any())
        {
            suggestions.Add("All participants are heavily booked. Consider rescheduling some existing meetings.");
        }

        suggestions.Add($"Meetings are restricted to {MeetingConstants.Meeting.WorkingHoursStart:00}:00 - " +
                       $"{MeetingConstants.Meeting.WorkingHoursEnd:00}:00. Consider adjusting if needed.");

        return suggestions;
    }

    private int FindLargestGap(
        List<Meeting> conflictingMeetings,
        DateTime earliestStart,
        DateTime latestEnd)
    {
        if (!conflictingMeetings.Any())
            return (int)(latestEnd - earliestStart).TotalMinutes;

        var gaps = new List<int>();
        if (conflictingMeetings.First().StartTime.HasValue)
        {
            gaps.Add((int)(conflictingMeetings.First().StartTime.Value - earliestStart).TotalMinutes);
        }

        for (int i = 0; i < conflictingMeetings.Count - 1; i++)
        {
            if (conflictingMeetings[i].EndTime.HasValue && 
                conflictingMeetings[i + 1].StartTime.HasValue)
            {
                var gap = (int)(conflictingMeetings[i + 1].StartTime.Value - 
                                conflictingMeetings[i].EndTime.Value).TotalMinutes;
                gaps.Add(gap);
            }
        }

        if (conflictingMeetings.Last().EndTime.HasValue)
        {
            gaps.Add((int)(latestEnd - conflictingMeetings.Last().EndTime.Value).TotalMinutes);
        }
        
        return gaps.Max();
    }
}