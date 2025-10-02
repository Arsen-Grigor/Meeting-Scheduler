using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Infrastructure.Persistence;
// auto-generated
public static class AppDbContextSeed
{
    public static async Task SeedSampleDataAsync(AppDbContext context)
    {
        if (context.Meetings.Any() || context.Participants.Any())
        {
            return;
        }

        var meeting1Id = Guid.NewGuid();
        var meeting2Id = Guid.NewGuid();
        var meeting3Id = Guid.NewGuid();
        var participant1Id = Guid.NewGuid();
        var participant2Id = Guid.NewGuid();
        var participant3Id = Guid.NewGuid();
        var participant4Id = Guid.NewGuid();
        var participant5Id = Guid.NewGuid();
        var participants = new[]
        {
            Participant.Create(
                participant1Id,
                "John Smith",
                "CEO",
                "john.smith@company.com"),
            
            Participant.Create(
                participant2Id,
                "Sarah Johnson",
                "Product Manager",
                "sarah.johnson@company.com"),
            
            Participant.Create(
                participant3Id,
                "Michael Brown",
                "Software Architect",
                "michael.brown@company.com"),
            
            Participant.Create(
                participant4Id,
                "Emily Davis",
                "Team Lead",
                "emily.davis@company.com"),
            
            Participant.Create(
                participant5Id,
                "David Wilson",
                "Software Engineer",
                "david.wilson@company.com")
        };

        var meetings = new[]
        {
            Meeting.Create(
                meeting1Id,
                "Q4 Strategy Planning",
                "Quarterly strategy planning session to discuss goals and objectives for Q4."),
            
            Meeting.Create(
                meeting2Id,
                "Architecture Review",
                "Technical architecture review for the new microservices migration project."),
            
            Meeting.Create(
                meeting3Id,
                "Sprint Planning",
                "Sprint planning meeting for the upcoming two-week sprint cycle.")
        };

        var now = DateTime.UtcNow;
        meetings[0].Schedule(
            now.AddDays(1).Date.AddHours(10), 
            now.AddDays(1).Date.AddHours(11).AddMinutes(30));
        
        meetings[1].Schedule(
            now.AddDays(2).Date.AddHours(14), 
            now.AddDays(2).Date.AddHours(15).AddMinutes(30));
        
        meetings[2].Schedule(
            now.AddDays(3).Date.AddHours(9), 
            now.AddDays(3).Date.AddHours(10));

        meetings[0].AddParticipant((ParticipantId)participant1Id);
        meetings[0].AddParticipant((ParticipantId)participant2Id);
        
        meetings[1].AddParticipant((ParticipantId)participant3Id);
        meetings[1].AddParticipant((ParticipantId)participant4Id);
        
        meetings[2].AddParticipant((ParticipantId)participant5Id);
        meetings[2].AddParticipant((ParticipantId)participant4Id);

        await context.Participants.AddRangeAsync(participants);
        await context.Meetings.AddRangeAsync(meetings);
        
        await context.SaveChangesAsync();
    }
}