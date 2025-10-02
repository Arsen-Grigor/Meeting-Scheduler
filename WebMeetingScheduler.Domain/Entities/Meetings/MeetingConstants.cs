namespace WebMeetingScheduler.Domain.Entities.Meetings;

public class MeetingConstants
{
    public class Meeting{
        public const int MeetingTitleMinLength = 3;
        public const int MeetingTitleMaxLength = 150;
        public const int MeetingDescriptionMaxLength = 500;
        
        public const int MinimumDurationMinutes = 15;
        public const int MaximumDurationMinutes = 180;
        
        public const int OverlapToleranceMinutes = 15;
        public const int WorkingHoursStart = 9;
        public const int WorkingHoursEnd = 17;
        public const int MaximumSchedulingDaysInAdvance = 90;
        public const int MinimumSchedulingAdvanceMinutes = 30;
    }
}