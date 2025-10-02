using Microsoft.EntityFrameworkCore;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;
using WebMeetingScheduler.Infrastructure.Exceptions;

namespace WebMeetingScheduler.Infrastructure.Persistence.Repositories;

public class MeetingsRepository : IMeetingsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMeetingScheduler _meetingScheduler;

    public MeetingsRepository(AppDbContext dbContext, IMeetingScheduler meetingScheduler)
    {
        _dbContext = dbContext;
        _meetingScheduler = meetingScheduler;
    }
    
    public async Task CreateMeetingAsync(
        Meeting meeting,
        int duration,
        DateTime tryEarliestStart,
        DateTime tryLatestEnd,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(meeting, cancellationToken);
        if (meeting.StartTime == default || meeting.EndTime == default)
        {
            var allMeetings = await GetAllMeetingsAsync(cancellationToken);
            var participantIds = meeting.ParticipantsId.ToList();
            var earliestStart = tryEarliestStart;
            var latestEnd = tryLatestEnd;
            var durationMinutes = duration;
            
            try
            {
                var suggestedStartTime = _meetingScheduler.TryScheduleMeeting(
                    allMeetings ?? new List<Meeting>(),
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd);
                
                if (suggestedStartTime.HasValue)
                {
                    meeting.Schedule(
                        suggestedStartTime.Value, 
                        suggestedStartTime.Value.AddMinutes(durationMinutes));
                }
            }
            catch (NoAvailableTimeSlotException)
            {
                // logged, ignore throw
            }
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateMeetingAsync(
        MeetingId meetingId,
        MeetingTitle? newTitle = null,
        MeetingDescription? newDescription = null,
        CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.Meetings
            .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

        if (meeting == null)
            throw new MeetingNotFoundException(meetingId.Value);

        meeting.UpdateTextData(title: newTitle, description: newDescription);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteMeetingAsync(MeetingId meetingId, CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.Meetings
            .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

        if (meeting == null)
            throw new MeetingNotFoundException(meetingId.Value);

        _dbContext.Meetings.Remove(meeting);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddParticipantToMeetingAsync(
        ParticipantId participantId, 
        MeetingId meetingId,
        CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.Meetings
            .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

        if (meeting == null)
            throw new MeetingNotFoundException(meetingId.Value);

        meeting.AddParticipant(participantId);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveParticipantFromMeetingAsync(
        ParticipantId participantId, 
        MeetingId meetingId,
        CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.Meetings
            .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

        if (meeting == null)
            throw new MeetingNotFoundException(meetingId.Value);

        meeting.RemoveParticipant(participantId);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<Meeting?> GetMeetingAsync(
        MeetingId meetingId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Meetings
            .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);
    }
    
    public async Task<List<Meeting>?> GetAllMeetingsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Meetings.ToListAsync(cancellationToken);
    }
}