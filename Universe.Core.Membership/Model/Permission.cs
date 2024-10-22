using Universe.Core.Data;

namespace Universe.Core.Membership.Model
{
    public class Permission : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
