using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.Interfaces.Repositories.Users;

namespace FinanceControl.Services.Users;

public class UserAppService(
    IUserRepository repository,
    IUserDomService domService) : IUserAppService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindByEmailAndPasswordAsync(request.Email, request.Password, cancellationToken);
        return domService.Login(user);
    }

    public async Task<UserDto> RegisterAsync(UserRegisterDto dto, CancellationToken cancellationToken = default)
    {
        domService.ValidateRegister(dto);
        if (await repository.EmailExistsAsync(dto.Email.Trim(), cancellationToken))
            throw new InvalidOperationException("E-mail já cadastrado.");

        var entity = domService.CreateFromRegister(dto);
        await repository.AddAsync(entity, cancellationToken);
        return (await repository.GetByIdAsync(entity.UserId, cancellationToken))!;
    }

    public Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default) =>
        repository.FilterAsync(filter, cancellationToken);

    public Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<UserDto?> UpdateAsync(UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await repository.FindTrackedAsync(dto.UserId, cancellationToken);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync(cancellationToken);
        return await repository.GetByIdAsync(entity.UserId, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
