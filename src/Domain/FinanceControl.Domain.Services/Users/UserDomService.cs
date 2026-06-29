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
            Email = user.UserEmail ?? string.Empty,
            SecurityStamp = user.SecurityStamp,
            Moeda = user.Currency,
            Idioma = user.Language,
            FormatoData = user.DateFormat,
            InicioMes = user.FinancialMonthStartDay
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

    public void ApplyFinancialPreferences(User entity, UserFinancialPreferencesDto dto)
    {
        ValidateFinancialPreferences(dto);
        UserMapper.ApplyFinancialPreferences(entity, dto);
    }

    public void ValidateFinancialPreferences(UserFinancialPreferencesDto dto)
    {
        if (!FinancialPreferenceDefaults.MoedasValidas.Contains(dto.Moeda))
            throw new ArgumentException("Moeda inválida.");
        if (!FinancialPreferenceDefaults.IdiomasValidos.Contains(dto.Idioma))
            throw new ArgumentException("Idioma inválido.");
        if (!FinancialPreferenceDefaults.FormatosDataValidos.Contains(dto.FormatoData))
            throw new ArgumentException("Formato de data inválido.");
        if (!FinancialPreferenceDefaults.IniciosMesValidos.Contains(dto.InicioMes))
            throw new ArgumentException("Início do mês financeiro inválido.");
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

    public void ValidateDeleteAccount(User entity, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Informe sua senha para confirmar a exclusão da conta.");
        if (!string.Equals(entity.Password, password, StringComparison.Ordinal))
            throw new ArgumentException("Senha incorreta. A exclusão não foi realizada.");
    }
}
