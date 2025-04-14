namespace CashbackTicket.Models
{
    // Dtos/CheckoutDto.cs
    public class CheckoutDto
    {
        public int GiftCardId { get; set; }
        public int PaymentMethodId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
    }

    // Dtos/VerifyDto.cs
    public class VerifyDto
    {
        public string PromoCode { get; set; }
    }

    // Dtos/VerifyResponse.cs
    public class VerifyResponse
    {
        public bool Valid { get; set; }
        public decimal Amount { get; set; }
        public string GiftCardId { get; set; }
    }

    // Dtos/UsePromoDto.cs
    public class UsePromoDto
    {
        public string PromoCode { get; set; }
    }

    // Dtos/RefreshTokenDto.cs
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
