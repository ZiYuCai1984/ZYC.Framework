using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ZYC.Framework.Modules.BlazorDemo;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("out")]
    public async Task<IResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Results.Redirect("/Account/Login?ReturnUrl=%2F");
    }
}