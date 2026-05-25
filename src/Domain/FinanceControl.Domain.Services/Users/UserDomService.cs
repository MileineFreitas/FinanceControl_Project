using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.MapperProfiles.Users;

namespace FinanceControl.Domain.Services.Users;

public class UserDomService : IUserDomService
{
    public LoginResponseDto? Login(User? user)
    {
        if (user == null) return null;

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Name = user.UserName ?? string.Empty,
            Email = user.UserEmail ?? string.Empty
        };
    }

    public User CreateFromRegister(UserRegisterDto dto)
    {
        ValidateRegister(dto);
        return UserMapper.ToEntity(dto);
    }

    public void ApplyUpdate(User entity, UserUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName))
            throw new ArgumentException("Nome de usuário é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("E-mail é obrigatório.");
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                throw new ArgumentException("Informe a senha atual para confirmar a alteração.");
            if (!string.Equals(entity.Password, dto.CurrentPassword, StringComparison.Ordinal))
                throw new ArgumentException("A senha atual informada está incorreta.");
        }

        UserMapper.ApplyUpdate(entity, dto);
    }

    public void ValidateRegister(UserRegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) || dto.UserName.Trim().Length < 3)
            throw new ArgumentException("Nome de usuário deve ter pelo menos 3 caracteres.");
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("E-mail é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            throw new ArgumentException("Senha deve ter pelo menos 8 caracteres.");
    }
}
