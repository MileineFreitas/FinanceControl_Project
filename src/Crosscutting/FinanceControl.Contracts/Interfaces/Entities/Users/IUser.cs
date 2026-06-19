using FinanceControl.Contracts.Enumerators.Users;

namespace FinanceControl.Contracts.Interfaces.Entities.Users;

public interface IUser
{
    Guid UserId { get; set; }

    string? UserName { get; set; }

    string? UserEmail { get; set; }

    string? Password { get; set; }

    string? ProfilePhoto { get; set; }

    bool IsActive { get; set; }

    DateTime DateCreated { get; set; }
}
