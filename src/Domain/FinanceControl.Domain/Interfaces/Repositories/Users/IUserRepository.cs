using System.Collections.Generic;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Interfaces.Repositories.Users;

public interface IUserRepository
{
    IEnumerable<User> GetAllUsers();

    User GetUserById(int id);

    User CreateUser(User user);

    User UpdateUser(User user);

    User DeleteUser(int id);
}
