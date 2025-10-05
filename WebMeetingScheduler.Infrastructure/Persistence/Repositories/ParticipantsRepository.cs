using Microsoft.EntityFrameworkCore;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;
using WebMeetingScheduler.Infrastructure.Exceptions;

namespace WebMeetingScheduler.Infrastructure.Persistence.Repositories;


public class ParticipantsRepository : IParticipantsRepository
{
    private readonly AppDbContext _dbContext;

    public ParticipantsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesToDbContextAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task CreateParticipantAsync(
        Participant participant, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(participant, cancellationToken);
    }

    public async Task AddMeetingAsync(
        ParticipantId participantId,
        MeetingId meetingId,
        CancellationToken cancellationToken = default)
    {
        var participant = await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        if (participant == null)
            throw new ParticipantNotFoundException(participantId.Value);

        participant.AddMeeting(meetingId);
    }

    public async Task RemoveMeetingAsync(
        ParticipantId participantId,
        MeetingId meetingId,
        CancellationToken cancellationToken = default)
    {
        var participant = await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        if (participant == null)
            throw new ParticipantNotFoundException(participantId.Value);

        participant.RemoveMeeting(meetingId);
    }

    public async Task UpdateParticipantAsync(
        ParticipantId participantId,
        FullName? fullName = null,
        Roles? role = null,
        Email? email = null,
        CancellationToken cancellationToken = default)
    {
        var participant = await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        if (participant == null)
            throw new ParticipantNotFoundException(participantId.Value);
        
        participant.UpdateData(fullName, role, email);
    }
    
    public async Task DeleteParticipantAsync(
        ParticipantId participantId, 
        CancellationToken cancellationToken = default)
    {
        var participant = await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        if (participant == null)
            throw new ParticipantNotFoundException(participantId.Value);

        _dbContext.Participants.Remove(participant);
    }
    
    public async Task<Participant?> GetParticipantByIdAsync(
        ParticipantId participantId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);
    }
    
    public async Task<List<Participant>?> GetAllParticipantsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Participants.ToListAsync(cancellationToken);
    }
    
    public async Task<List<Meeting>?> GetParticipantMeetingsAsync(
        ParticipantId participantId, 
        CancellationToken cancellationToken = default)
    {
        var participant = await _dbContext.Participants
            .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        if (participant == null)
            throw new ParticipantNotFoundException(participantId.Value);

        var meetingIds = participant.Meetings.ToList();
        
        var meetings = await _dbContext.Meetings
            .Where(m => meetingIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        return meetings;
    }
}