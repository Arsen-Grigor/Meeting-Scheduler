using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;

namespace WebMeetingScheduler.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => (ParticipantId)value)
            .IsRequired()
            .HasColumnName("ParticipantId")
            .HasColumnType("char(36)");
        builder.Property(p => p.fullName)
            .HasConversion(
                fullName => fullName.Value,
                value => (FullName)value)
            .IsRequired()
            .HasMaxLength(ParticipantConstants.Participant.FullNameMaxLength)
            .HasColumnName("FullName");
        builder.Property(p => p.Role)
            .HasConversion(
                role => role.Code,
                code => Roles.From(code))
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Role");
        builder.Property(p => p.Email)
            .HasConversion(
                email => email.Value,
                value => (Email)value)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("Email");

        var meetingsConverter = new ValueConverter<List<MeetingId>, string>(
            v => string.Join(",", v.Select(m => m.Value.ToString()).ToArray()),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<MeetingId>()
                : v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => (MeetingId)Guid.Parse(id))
                    .ToList()
        );

        builder.Property<List<MeetingId>>("_meetings")
            .HasColumnName("MeetingIds")
            .HasMaxLength(4000)
            .HasConversion(meetingsConverter);
        builder.Ignore(p => p.Meetings);
        builder.Ignore(p => p.DomainEvents);
        builder.HasIndex(p => p.Email)
            .IsUnique();
        builder.HasIndex(p => p.fullName);
    }
}