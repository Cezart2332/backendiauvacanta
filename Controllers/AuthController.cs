using Microsoft.AspNetCore.Mvc;
using IauVacanta.Backend.DTOs;
using IauVacanta.Backend.Interfaces;
using IauVacanta.Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace IauVacanta.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
        {
            var user = await _authService.Register(request);
            if (user == null)
            {
                return Conflict("Username or email already exists.");
            }

            var accessToken = await _authService.CreateToken(user);
            var refreshToken = await _authService.GenerateRefreshToken(user.Id);
            SetAccessToken(accessToken);
            SetRefreshToken(refreshToken);

            return Ok(new AuthResponseDto
            {
                User = _authService.MapUser(user)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            var user = await _authService.ValidateCredentials(request);
            if (user == null)
            {
                return Unauthorized("Wrong credentials.");
            }

            var accessToken = await _authService.CreateToken(user);
            var refreshToken = await _authService.GenerateRefreshToken(user.Id);
            SetAccessToken(accessToken);
            SetRefreshToken(refreshToken);

            return Ok(new AuthResponseDto
            {
                User = _authService.MapUser(user)
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByRefreshToken(refreshToken);
            if (user == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            await _authService.RevokeRefreshToken(refreshToken);
            var accessToken = await _authService.CreateToken(user);
            var newRefreshToken = await _authService.GenerateRefreshToken(user.Id);
            SetAccessToken(accessToken);
            SetRefreshToken(newRefreshToken);

            return Ok(new AuthResponseDto
            {
                User = _authService.MapUser(user)
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeRefreshToken(refreshToken);
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

        private void SetAccessToken(string accessToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true
            };
            Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        }
    }
}