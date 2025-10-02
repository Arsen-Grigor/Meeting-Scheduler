using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Exceptions;

namespace WebMeetingScheduler.Domain.Entities.Participants;

public class Roles : ValueObject
{
    static Roles()
    {
    }

    private Roles()
    {
    }

    private Roles(string code)
    {
        Code = code;
    }

    public static Roles From(string code)
    {
        var role = new Roles { Code = code };

        if (!SupportedRoles.Any(r => r.Code == code))
        {
            throw new UnsupportedRoleException(code);
        }

        return role;
    }

    public static Roles NoRoles => new("No Roles");
    public static Roles Ceo => new("CEO");
    public static Roles ProductManager => new("Product Manager");
    public static Roles SoftwareArchitect => new("Software Architect");
    public static Roles TeamLead => new ("Team Lead");
    public static Roles SoftwareEngineer => new("Software Engineer");
    public static Roles SoftwareDeveloper => new("SoftwareDeveloper");
    public static Roles BusinessAccountManage => new ("Business Account Manage");
    public static Roles HrSpecialist => new("HR Specialist");
    public static Roles SocialMediaManager => new ("Social Media Manager");
    public static Roles BigDataEngineer => new ("Big Data Engineer");
    public static Roles DevOpsEngineer => new("DevOpsEngineer");
    public static Roles DataScientist => new("Data Scientist");

    public string Code { get; private set; } = "No Role";

    public static implicit operator string(Roles roles) => roles?.Code;
    public static implicit operator Roles(string code) => code is null ? null : From(code);


    public override string ToString()
    {
        return Code;
    }

    protected static IEnumerable<Roles> SupportedRoles
    {
        get
        {
            yield return Ceo;
            yield return ProductManager;
            yield return SoftwareArchitect;
            yield return TeamLead;
            yield return SoftwareEngineer;
            yield return SoftwareDeveloper;
            yield return BusinessAccountManage;
            yield return HrSpecialist;
            yield return SocialMediaManager;
            yield return BigDataEngineer;
            yield return DevOpsEngineer;
            yield return DataScientist;
            yield return NoRoles;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}