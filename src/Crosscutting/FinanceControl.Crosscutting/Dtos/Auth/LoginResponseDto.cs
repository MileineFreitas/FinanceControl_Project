using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Contracts.Dtos.Auth;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
