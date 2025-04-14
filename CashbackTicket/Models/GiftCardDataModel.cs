using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashbackTicket.Models
{
    [Table("GiftCardData")]
    public class GiftCardData
    {
        [Key]
        public string GiftCardId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? GiftCardNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethodId { get; set; }
        public int Quantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? Active { get; set; }
        public int? GiftPerUserLimit { get; set; }
        public int? MaxLimitToBuy { get; set; }
        public decimal? Discount { get; set; }
    }

    public class GiftCardDataModel
    {

        public string GiftCardId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string? PaymentMethodId { get; set; }

        [Required]
        public int Quantity { get; set; }
        public int? GiftPerUserLimit { get; set; }
        public int? MaxLimitToBuy { get; set; }
        public decimal? Discount { get; set; }
    }
}
