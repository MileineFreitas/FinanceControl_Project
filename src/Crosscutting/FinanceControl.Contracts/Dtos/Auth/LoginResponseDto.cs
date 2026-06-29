using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Contracts.Dtos.Auth;

public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;    
    public string Email { get; set; } = string.Empty;
    public Guid SecurityStamp { get; set; }
}
