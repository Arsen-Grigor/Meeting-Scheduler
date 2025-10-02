namespace WebMeetingScheduler.Infrastructure.Exceptions;

public class NoAvailableTimeSlotException : InfrastructureException
{
    public int RequestedDurationMinutes { get; }
    public DateTime EarliestStart { get; }
    public DateTime LatestEnd { get; }
    public List<string> Suggestions { get; }

    public NoAvailableTimeSlotException(
        int requestedDurationMinutes,
        DateTime earliestStart,
        DateTime latestEnd,
        List<string> suggestions)
        : base(BuildMessage(requestedDurationMinutes, earliestStart, latestEnd, suggestions))
    {
        RequestedDurationMinutes = requestedDurationMinutes;
        EarliestStart = earliestStart;
        LatestEnd = latestEnd;
        Suggestions = suggestions;
    }

    private static string BuildMessage(
        int duration, 
        DateTime earliest, 
        DateTime latest, 
        List<string> suggestions)
    {
        var message = $"No available time slot found for a {duration}-minute meeting " +
                      $"between {earliest:yyyy-MM-dd HH:mm} and {latest:yyyy-MM-dd HH:mm}.";

        if (suggestions.Any())
        {
            message += "\n\nSuggestions:\n- " + string.Join("\n- ", suggestions);
        }

        return message;
    }
}