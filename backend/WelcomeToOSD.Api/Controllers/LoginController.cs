using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace WelcomeToOSD.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController() : ControllerBase
{
    [HttpPost]
    public IActionResult Login(LoginRequest request)
    {
        if (request.Email != "test@test.com" || request.Password != "password")
            return Unauthorized();

        return Ok();
    }
}
