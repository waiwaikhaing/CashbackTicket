using CashbackTicket.EFDBContext;
using CashbackTicket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Security.Cryptography;
using static CashbackTicket.Services.UserService;
using System.Text;

namespace CashbackTicket.Services
{
    public interface IUserService
    {
        Task<ServiceResponse> UserRegister(UserRegisterDTO dto);
        Task<TokenResponse> GenerateToken(UserDTO dto);
    }

    public class UserService : IUserService
    {
        private readonly AppDBContext _context;
        private readonly IJwtService _jwtService;
        public UserService(AppDBContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<ServiceResponse> UserRegister(UserRegisterDTO dto)
        {
            var userData = await _context.Users.FirstOrDefaultAsync(u => u.Username == 
            dto.Username);
            if (userData != null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }
            var userDataPhno = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber ==
           dto.PhoneNumber);
            if (userDataPhno != null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Phone number already exists"
                };
            }

            var user = new UserData
            {
                UserId = Guid.NewGuid().ToString(),
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Active = true,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.NewGuid().ToString()
            };

            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Registration successful"
            };
        }

        public async Task<TokenResponse> GenerateToken(UserDTO dto)
        {
            // Validate user credentials
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }
            var secureKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            
            // Generate JWT token
            var token = await _jwtService.GenerateToken(user);

            if (token != null)
            {
                //update token
                user.CurrentToken = token;
                user.RefreshToken = Guid.NewGuid().ToString();
                user.RefreshTokenExpiryTime = DateTime.Now;
                var result = await _context.SaveChangesAsync();
            }

            return new TokenResponse
            {
                Success = true,
                Message = "Token generated successfully",
                Token = token
            };
        }

        //public async Task<AuthResponse> RefreshToken(string token, string refreshToken)
        //{
        //    var principal = GetPrincipalFromExpiredToken(token);
        //    var username = principal.Identity.Name;
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        //    if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        //        return new AuthResponse { Success = false, Message = "Invalid token" };

        //    var newToken = await GenerateToken(user);
        //    var newRefreshToken = GenerateRefreshToken();

        //    user.RefreshToken = newRefreshToken;
        //   user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(15);

        //    await _context.SaveChangesAsync();

        //    return new AuthResponse
        //    {
        //        Success = true,
        //        Token = newToken.Token,
        //        RefreshToken = newRefreshToken
        //    };
        //}


        ///////////////////////////////
        ///
       

            // ... [previous methods remain the same] ...

            //private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
            //{
            //    var tokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = false,
            //        ValidateIssuer = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = _key,
            //        ValidateLifetime = false // we want to get claims from expired token
            //    };

            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    SecurityToken securityToken;
            //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            //    var jwtSecurityToken = securityToken as JwtSecurityToken;

            //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            //        throw new SecurityTokenException("Invalid token");

            //    return principal;
            //}

            //private string GenerateRefreshToken()
            //{
            //    var randomNumber = new byte[32];
            //    using (var rng = RandomNumberGenerator.Create())
            //    {
            //        rng.GetBytes(randomNumber);
            //        return Convert.ToBase64String(randomNumber);
            //    }
            //}

        //    private string GenerateJwtToken(User user)
        //    {
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Name, user.Username),
        //    new Claim(ClaimTypes.Role, user.Role)
        //};

        //        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.UtcNow.AddMinutes(15),
        //            SigningCredentials = creds
        //        };

        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var token = tokenHandler.CreateToken(tokenDescriptor);

        //        return tokenHandler.WriteToken(token);
        //    }

            //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
            //{
            //    using (var hmac = new HMACSHA512())
            //    {
            //        passwordSalt = hmac.Key;
            //        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            //    }
            //}

            //private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
            //{
            //    using (var hmac = new HMACSHA512(storedSalt))
            //    {
            //        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            //        for (int i = 0; i < computedHash.Length; i++)
            //        {
            //            if (computedHash[i] != storedHash[i]) return false;
            //        }
            //    }
            //    return true;
            //}

            //public async Task<AuthResponse> Register(UserDto userDto)
            //{
            //    if (await UserExists(userDto.Username))
            //        return new AuthResponse { Success = false, Message = "Username already exists" };

            //    byte[] passwordHash, passwordSalt;
            //    CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);

            //    var user = new User
            //    {
            //        Username = userDto.Username,
            //        PasswordHash = passwordHash,
            //        PasswordSalt = passwordSalt,
            //        Role = "User" // Default role
            //    };

            //    _context.Users.Add(user);
            //    await _context.SaveChangesAsync();

            //    return new AuthResponse { Success = true, Message = "User registered successfully" };
            //}

            //public async Task<bool> UserExists(string username)
            //{
            //    return await _context.Users.AnyAsync(x => x.Username == username);
            //}
        

    }
}
