using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.MapperProfiles.Users;

public static class UserMapper
{
    public static UserDto ToDto(User entity) =>
        new()
        {
            UserId = entity.UserId,
            UserName = entity.UserName,
            UserEmail = entity.UserEmail,
            ProfilePhoto = entity.ProfilePhoto,
            IsActive = entity.IsActive,
            Moeda = entity.Currency,
            Idioma = entity.Language,
            FormatoData = entity.DateFormat,
            InicioMes = entity.FinancialMonthStartDay
        };

    public static UserFinancialPreferencesDto ToPreferencesDto(User entity) =>
        new()
        {
            Moeda = entity.Currency,
            Idioma = entity.Language,
            FormatoData = entity.DateFormat,
            InicioMes = entity.FinancialMonthStartDay
        };

    public static User ToEntity(UserRegisterDto dto) =>
        new()
        {
            UserName = dto.UserName.Trim(),
            UserEmail = dto.Email.Trim(),
            Password = dto.Password,
            ProfilePhoto = dto.ProfilePhoto,
            DateCreated = DateTime.UtcNow,
            IsActive = true,
            SecurityStamp = Guid.NewGuid(),
            Currency = FinancialPreferenceDefaults.Moeda,
            Language = FinancialPreferenceDefaults.Idioma,
            DateFormat = FinancialPreferenceDefaults.FormatoData,
            FinancialMonthStartDay = FinancialPreferenceDefaults.InicioMes
        };

    public static void ApplyUpdate(User entity, UserUpdateDto dto)
    {
        entity.UserName = dto.UserName.Trim();
        entity.UserEmail = dto.Email.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Password))
            entity.Password = dto.Password;
        if (dto.ProfilePhoto != null)
            entity.ProfilePhoto = dto.ProfilePhoto;
        entity.IsActive = dto.IsActive;
    }

    public static void ApplyFinancialPreferences(User entity, UserFinancialPreferencesDto dto)
    {
        entity.Currency = dto.Moeda;
        entity.Language = dto.Idioma;
        entity.DateFormat = dto.FormatoData;
        entity.FinancialMonthStartDay = dto.InicioMes;
    }
}
