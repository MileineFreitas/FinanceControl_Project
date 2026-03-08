using System.Collections.Generic;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.Users;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
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

    [HttpPost]
    public ActionResult CreateUser(User user)
    {
        if (user == null)
        {
            return BadRequest();
        }

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
