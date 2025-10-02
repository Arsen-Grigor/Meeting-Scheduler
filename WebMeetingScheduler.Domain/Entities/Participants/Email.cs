using System.Text.RegularExpressions;
using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Participants
{
    public sealed record Email
    {
        public string Value { get; set; }
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            {
                throw new InvalidEmailDomainException(email);
            }

            Value = email;
        }
        
        public static explicit operator string(Email email) => email?.Value;
        public static implicit operator Email(string value) => value is null ? null : new Email(value);
    }
}