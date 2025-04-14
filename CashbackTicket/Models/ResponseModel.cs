namespace CashbackTicket.Models
{
    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ServiceDataResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class TokenResponse : ServiceResponse
    {
        public string Token { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }        // JWT access token
        public string RefreshToken { get; set; } // Refresh token
        public bool Success { get; set; }        // Operation status
        public string Message { get; set; }      // Additional info (errors, etc.)
    }


    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
