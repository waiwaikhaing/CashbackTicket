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
using Microsoft.AspNetCore.Mvc;

namespace CashbackTicket.Services
{
    public interface IUserService
    {
        Task<ServiceResponse> UserRegister(UserRegisterDTO dto);
        Task<TokenResponse> GenerateToken(UserDTO dto);
        Task<ServiceDataResponse<UserData>> GetUserByUserID(string userId);
    }

    public class UserService : IUserService
    {
        private readonly AppDBContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;
        public UserService(AppDBContext context, IJwtService jwtService, IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
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
                user.RefreshToken = await _jwtService.GenerateRefreshToken();//Guid.NewGuid().ToString();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:RefreshTokenExpirationDays"]));
                var result = await _context.SaveChangesAsync();
            }

            return new TokenResponse
            {
                Success = true,
                Message = "Token generated successfully",
                Token = token
            };
        }

     
        public async Task<ServiceDataResponse<UserData>> GetUserByUserID(string userId)
        {
            ServiceDataResponse<UserData> response = new ServiceDataResponse<UserData>();
            try
            {
                var result = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                if (result != null)
                {
                    response.Success = true;
                    response.Message = "Success";
                    response.Data = result;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No Data";
                }
            }
            catch (Exception ex) { 

            }
            return response;
        }
    }





}
