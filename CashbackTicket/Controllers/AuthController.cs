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

        public AuthController(IUserService userService)
        {
            _userService = userService;
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

        //public async Task<AuthResponse> RefreshToken(string token, string refreshToken)
        //{
        //    var principal = GetPrincipalFromExpiredToken(token);
        //    var username = principal.Identity.Name;
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        //    if (user == null || user.RefreshToken != refreshToken || user.TokenExpires <= DateTime.UtcNow)
        //        return new AuthResponse { Success = false, Message = "Invalid token" };

        //    var newToken = GenerateJwtToken(user);
        //    var newRefreshToken = GenerateRefreshToken();

        //    user.RefreshToken = newRefreshToken;
        //    user.TokenExpires = DateTime.UtcNow.AddMinutes(15);

        //    await _context.SaveChangesAsync();

        //    return new AuthResponse
        //    {
        //        Success = true,
        //        Token = newToken,
        //        RefreshToken = newRefreshToken
        //    };
        //}

    }
}
