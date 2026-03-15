using System;
using System.Collections.Generic;
using System.Text;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Domain.Interfaces.AppServices.Users;

namespace FinanceControl.Domain.Services.Users;

public class UserDomService : IUserAppService
{
    private readonly IUserRepository _repository;

    public UserDomService(IUserRepository repository)
    {
        _repository = repository;
    }

    public LoginResponseDto? Login(LoginRequestDto request)
    {
        var user = _repository.GetUserByEmailAndPassword(request.Email, request.Password);
        if (user == null)
        {
            return null;
        }

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Name = user.UserName ?? string.Empty,
            Email = user.UserEmail ?? string.Empty 
        };
    }
}