namespace FinanceControl.Contracts.Dtos.Users;

public class UserDto
{
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }

    public string? ProfilePhoto { get; set; }

    public bool IsActive { get; set; }
}
