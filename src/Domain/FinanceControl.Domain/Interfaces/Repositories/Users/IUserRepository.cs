using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Interfaces.Repositories.Users;

public interface IUserRepository
{
    Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter);

    Task<UserDto?> GetByIdAsync(int id);

    Task<User> AddAsync(User user);

    Task<User?> FindTrackedAsync(int id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(int id);

    Task<User?> FindByEmailAndPasswordAsync(string email, string password);

    Task<bool> EmailExistsAsync(string email);
}
