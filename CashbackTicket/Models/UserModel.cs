using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CashbackTicket.Models
{
    [Table("UserData")]
    public class UserData
    {
        [Key]
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string? CurrentToken { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool Active { get; set; }

    }

    public class UserDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Username must be between 10 and 50 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character.")]
        public string Password { get; set; }
        //[Required]
        //public string PhoneNumber { get; set; }

    }

    public class UserRegisterDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Username must be between 10 and 50 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character.")]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}
