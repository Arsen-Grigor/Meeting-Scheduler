using WebMeetingScheduler.Application.Common;

namespace WebMeetingScheduler.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}