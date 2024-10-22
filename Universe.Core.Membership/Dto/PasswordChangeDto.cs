using Universe.Core.Infrastructure;

namespace Universe.Core.Membership.Dto;

public class PasswordChangeDto : IDto
{
    public int Id { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; }

    public Guid UserId { get; set; }
}
