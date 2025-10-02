namespace WebMeetingScheduler.Infrastructure.Settings;

public record AppSettings
{
    public Authentication Authentication { get; init; } = null!;
    public ConnectionStrings ConnectionStrings { get; init; } = null!;
}

public record ConnectionStrings(string DefaultConnection);