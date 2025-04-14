using CashbackTicket.Models;
using CashbackTicket.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CashbackTicket.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;
        public PaymentController(
        IHttpContextAccessor httpContextAccessor, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }

        [HttpGet("GetAllPaymentMethod")]
        public async Task<IActionResult> GetAllPaymentMethod()
        {
            var userId = GetUserId();
            var result = await _paymentService.GetAllPaymentMethod(userId);
            return Ok(result);
        }
        

        [HttpPost("CreateOrEditPayment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentMethodModel model)
        {
            var userId = GetUserId();
            var result = await _paymentService.CreatePayment(model, userId); ;
            return Ok(result);
        }

        [HttpPost("PurchaseGiftCard")]
        public async Task<IActionResult> MakePayment(GiftCardPurchaseHistoryModel model)
        {
            var userId = GetUserId();
            var result = await _paymentService.MakePayment(model,userId);
            return Ok(result);
        }

        [HttpPost("VerifyPromoCode")]
        public async Task<ActionResult<VerifyResponse>> VerifyPromoCode(VerifyDto verifyDto)
        {
            var userId = GetUserId();
            var result = await _paymentService.VerifyPromoCode(verifyDto);
            return Ok(result);
        }

        [HttpPost("UsePromoCode")]
        public async Task<ActionResult<ServiceResponse>> UsePromoCode(VerifyDto verifyDto)
        {
            var userId = GetUserId();
            var result = await _paymentService.UsePromoCode(verifyDto);
            return Ok(result);

        }

    }
}
