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
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;
        return Ok(await appService.FilterAsync(filter));
    }

    [HttpGet("{id:guid}", Name = "ObterUsuario")]
    public async Task<IActionResult> GetUserId(Guid id)
    {
        var user = await appService.GetByIdAsync(id);
        return user == null ? NotFound("Usuario não encontrado...") : Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (request == null) return BadRequest("Requisição inválida...");
        var result = await appService.LoginAsync(request);
        return result == null ? Unauthorized("Email ou senha inválidos...") : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto dto)
    {
        if (dto == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = await appService.RegisterAsync(dto);
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

    [HttpPut("{id:guid}/user-update")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UserUpdateDto dto)
    {
        if (id != dto.UserId) return BadRequest("Id invalido...");
        try
        {
            var updated = await appService.UpdateAsync(dto);
            return updated == null ? NotFound("Usuario não encontrado...") : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await appService.DeleteAsync(id);
        return deleted ? Ok("Usuario excluido") : NotFound("Id invalido");
    }

    [HttpGet("{id:guid}/security-stamp")]
    public async Task<IActionResult> GetSecurityStamp(Guid id)
    {
        var stamp = await appService.GetSecurityStampAsync(id);
        return stamp == null ? NotFound("Usuario não encontrado...") : Ok(new { securityStamp = stamp.Value });
    }

    [HttpPost("{id:guid}/revoke-sessions")]
    public async Task<IActionResult> RevokeSessions(Guid id)
    {
        var stamp = await appService.RevokeOtherSessionsAsync(id);
        return stamp == null
            ? NotFound("Usuario não encontrado...")
            : Ok(new RevokeSessionsResponseDto { SecurityStamp = stamp.Value });
    }
}
