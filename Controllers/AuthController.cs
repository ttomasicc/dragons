using dragons.Dtos.User;
using dragons.Models;
using dragons.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using System;

namespace dragons.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(RegisterUserDto request)
        {
            var response = await _authService.Register(
                new User { Username = request.Username },
                request.Password
            );

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(LoginUserDto request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}