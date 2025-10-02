using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Infrastructure.Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => (MeetingId)value)
            .IsRequired()
            .HasColumnName("MeetingId")
            .HasColumnType("char(36)");
        builder.Property(m => m.Title)
            .HasConversion(
                title => title.Value,
                value => (MeetingTitle)value)
            .IsRequired()
            .HasMaxLength(MeetingConstants.Meeting.MeetingTitleMaxLength)
            .HasColumnName("Title");
        builder.Property(m => m.Description)
            .HasConversion(
                description => description.Value,
                value => (MeetingDescription)value)
            .IsRequired()
            .HasMaxLength(MeetingConstants.Meeting.MeetingDescriptionMaxLength)
            .HasColumnName("Description");
        builder.Property(m => m.StartTime)
            .HasColumnName("StartTime")
            .HasColumnType("datetime(6)");
        builder.Property(m => m.EndTime)
            .HasColumnName("EndTime")
            .HasColumnType("datetime(6)");
        builder.Ignore(m => m.Duration);
        // 
        var participantsConverter = new ValueConverter<List<ParticipantId>, string>(
            v => string.Join(",", v.Select(p => p.Value.ToString()).ToArray()),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<ParticipantId>()
                : v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => (ParticipantId)Guid.Parse(id))
                    .ToList()
        );

        builder.Property<List<ParticipantId>>("_participantsId")
            .HasColumnName("ParticipantIds")
            .HasMaxLength(4000)
            .HasConversion(participantsConverter);

        builder.Ignore(m => m.ParticipantsId);
        builder.Ignore(m => m.DomainEvents);
        builder.HasIndex(m => m.StartTime)
            .HasDatabaseName("IX_Meetings_StartTime");
        builder.HasIndex(m => m.EndTime)
            .HasDatabaseName("IX_Meetings_EndTime");
    }
}