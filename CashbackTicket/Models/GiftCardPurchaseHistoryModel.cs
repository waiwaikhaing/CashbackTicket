using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashbackTicket.Models
{
    [Table("GiftCardPurchaseHistory")]

    public class GiftCardPurchaseHistory
    {
        [Key]
        public string PurchaseId { get; set; }
        public string UserId { get; set; }
        public string GiftCardId { get; set; }
        public bool IsGift { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhoneNo { get; set; }
        public bool IsUsed { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? Active { get; set; }
        public DateTime? UsedOn { get; set; }
        public string? PromoCode { get; set; }
        public string? QRPath { get; set; }
        public string? ReceiverID { get; set; }
    }
    public class GiftCardPurchaseHistoryModel
        {
            public string? PurchaseId { get; set; }
           
            [Required]
            public string GiftCardId { get; set; }

            public bool IsGift { get; set; } = false;

        //[MaxLength(100)]
        //public string ReceiverName { get; set; }

        //[MaxLength(50)]
        //public string ReceiverPhoneNo { get; set; }
        public string? ReceiverID { get; set; }

        //public string PromoCode { get; set; }
        //public string QRPath { get; set; }
    }
    
    public class GiftCardDetailInformationView
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsGift { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedOn { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNo { get; set; }
        public string PromoCode { get; set; }
        public string QRPath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GiftCardNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal? Discount { get; set; }
        public int? GiftPerUserLimit { get; set; }
        public int? MaxLimitToBuy { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; } // Payment method name
        public decimal? PaymentDiscount { get; set; }
        public bool? GiftCardActive { get; set; }
        public string? ReceiveUserID { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string GiftCardId { get; set; }
    }
}

