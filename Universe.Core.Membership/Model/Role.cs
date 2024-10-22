using Universe.Core.Data;
using Microsoft.AspNetCore.Identity;

namespace Universe.Core.Membership.Model;

public class Role : IdentityRole<Guid>, IEntity<Guid>
{
    public bool TwoFactorRequired { get; set; }


    public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public IList<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
