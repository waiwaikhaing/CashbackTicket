using System.Security.Claims;
using CashbackTicket.Models;
using CashbackTicket.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashbackTicket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            //register user
            var result = await _userService.UserRegister(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken(UserDTO dto)
        {
            //generate token
            var result = await _userService.GenerateToken(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #region refresh token 
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request is null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid client request");
            }

            var principal = await _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token");
            }

            var user = await _userService.GetUserByUserID(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.Data == null ||
                user.Data.UserId != userId ||
                user.Data.RefreshToken != request.RefreshToken ||
                user.Data.RefreshTokenExpiryTime <= DateTime.UtcNow
                )
            {
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = _jwtService.GenerateToken(user.Data);
            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }
        #endregion

    }
}
