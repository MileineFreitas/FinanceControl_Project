using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinanceControl.API.Controllers.Users;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IUserAppService _userAppService;

    public UserController(IUserRepository repository, IUserAppService userAppService)
    {
        _repository = repository;
        _userAppService = userAppService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> Get()
    {
        var users = _repository.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterUsuario")]
    public ActionResult<User> GetUserId(int id)
    {
        var user = _repository.GetUserById(id);
        if (user == null)
        {
            return NotFound("Usuario não encontrado...");
        }
        return Ok(user);
    }


    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request)
    {
        if(request == null)
        {
            return BadRequest("Requisição inválida...");
        }
        var result = _userAppService.Login(request);
        if(result == null)
        {
            return Unauthorized("Email ou senha inválidos...");
        }
        return Ok(result);
    }


    [HttpPost("register")]
    public ActionResult CreateUser([FromBody] RegisterUserDto dto)
    {
        if (dto == null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            UserName = dto.UserName,
            UserEmail = dto.Email,
            Password = dto.Password,
            PhotoBase64 = dto.PhotoBase64
        };

        var createdUser = _repository.CreateUser(user);
        return new CreatedAtRouteResult("ObterUsuario",
            new { id = createdUser.UserId }, createdUser);
    }

    [HttpPut("{id:int:min(1)}/user-update")]
    public ActionResult Put(int id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest("Id invalido...");
        }

        _repository.UpdateUser(user);
        return Ok(user);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var user = _repository.DeleteUser(id);
        if (user == null)
        {
            return NotFound("Id invalido");
        }
        return Ok("Usuario excluido");
    }
}
