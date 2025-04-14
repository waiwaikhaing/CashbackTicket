using System.Security.Claims;
using CashbackTicket.Models;
using CashbackTicket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CashbackTicket.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GiftCardController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGiftCardService _giftCardService;
        public GiftCardController(
        IHttpContextAccessor httpContextAccessor, IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }

        [HttpGet("GetGiftCards")]
        public async Task<IActionResult> GetGiftCards()
        {
            var userId = GetUserId();
            var result = await _giftCardService.GetAllGiftCard();
            return Ok(result);
        }

        [HttpGet("GetGiftCardDetail/{id}")]
        public async Task<IActionResult> GetGiftCardDetail(string id)
        {
            var userId = GetUserId();
            var result = await _giftCardService.GetGiftCardDetail(id);
            return Ok(result);
        }

        [HttpPost("CreateOrEditGiftCard")]
        public async Task<IActionResult> CreateOrEditGiftCard([FromBody] GiftCardDataModel model)
        {
            var userId = GetUserId();
            var result = await _giftCardService.CreateOrEditGiftCard( model, userId);
            return Ok(result);
        }

        [HttpDelete("InactivateGiftCard/{cardId}")]
        public async Task<IActionResult> RemoveItem(string cardId)
        {
            var result = await _giftCardService.DeactivateGiftCard(cardId);
            return Ok(result);
        }

        [HttpGet("UnUsedCashbacks")]
        public async Task<IActionResult> UnUsedCashbacks()
        {
            var userId = GetUserId();
            var result = await _giftCardService.UnUsedCashbacks(userId);
            return Ok(result);
        }

        [HttpGet("UsedCashbacks")]
        public async Task<IActionResult> UsedCashbacks()
        {
            var userId = GetUserId();
            var result = await _giftCardService.UsedCashbacks(userId);
            return Ok(result);
        }
    }
}
