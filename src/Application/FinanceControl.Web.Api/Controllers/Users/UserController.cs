using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Users;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.Users;

[Route("[controller]")]
[ApiController]
public class UserController(IUserAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter, CancellationToken cancellationToken)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;
        return Ok(await appService.FilterAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterUsuario")]
    public async Task<IActionResult> GetUserId(int id, CancellationToken cancellationToken)
    {
        var user = await appService.GetByIdAsync(id, cancellationToken);
        return user == null ? NotFound("Usuario não encontrado...") : Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest("Requisição inválida...");
        var result = await appService.LoginAsync(request, cancellationToken);
        return result == null ? Unauthorized("Email ou senha inválidos...") : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto dto, CancellationToken cancellationToken)
    {
        if (dto == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = await appService.RegisterAsync(dto, cancellationToken);
            return CreatedAtRoute("ObterUsuario", new { id = created.UserId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}/user-update")]
    public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.UserId) return BadRequest("Id invalido...");
        try
        {
            var updated = await appService.UpdateAsync(dto, cancellationToken);
            return updated == null ? NotFound("Usuario não encontrado...") : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await appService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok("Usuario excluido") : NotFound("Id invalido");
    }
}
