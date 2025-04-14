using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashbackTicket.Models
{
    [Table("PaymentMethod")]
    public class PaymentMethod
    {
        [Key]
        public string? PaymentMethodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Discount { get; set; } = 0;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [MaxLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool Active { get; set; } = true;
    }

    public class PaymentMethodModel
    {
        public string? PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal Discount { get; set; } = 0;

    }
}
