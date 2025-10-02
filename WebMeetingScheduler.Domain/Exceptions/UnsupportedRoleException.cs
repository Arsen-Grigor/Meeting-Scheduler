namespace WebMeetingScheduler.Domain.Exceptions;

public class UnsupportedRoleException(string code)
    : Exception($"Role \"{code}\" is unsupported.");