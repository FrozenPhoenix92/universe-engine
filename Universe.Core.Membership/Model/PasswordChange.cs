using Universe.Core.Data;

namespace Universe.Core.Membership.Model;


public class PasswordChange : IEntity
{
    public int Id { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public Guid UserId { get; set; }
}
