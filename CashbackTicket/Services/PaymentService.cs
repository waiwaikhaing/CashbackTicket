using AutoMapper;
using Azure;
using CashbackTicket.EFDBContext;
using CashbackTicket.Helper;
using CashbackTicket.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.IO;
using System.Text.Json;

namespace CashbackTicket.Services
{
    public interface IPaymentService
    {
        Task<ServiceDataResponse<List<PaymentMethod>>> GetAllPaymentMethod(string userId);
        Task<ServiceResponse> MakePayment(GiftCardPurchaseHistoryModel model,string userId);
        Task<ServiceResponse> CreatePayment(PaymentMethodModel model, string userId);
        Task<ActionResult<VerifyResponse>> VerifyPromoCode(VerifyDto verifyDto);
        Task<ActionResult<ServiceResponse>> UsePromoCode(VerifyDto verifyDto);
    }
    public class PaymentService : IPaymentService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public PaymentService(AppDBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        #region get item
        public async Task<ServiceDataResponse<List<PaymentMethod>>> GetAllPaymentMethod(string userId)
        {
            ServiceDataResponse<List<PaymentMethod>> response = new ServiceDataResponse<List<PaymentMethod>>();
            List<PaymentMethod>? list = new List<PaymentMethod>();
            try
            {
                var result = await _context.PaymentMethods
                        .Where(v => v.Active == true)
                        .ToListAsync();

               // list = result != null ? _mapper.Map<List<PaymentMethod>, List<PaymentMethodModel>>(result) : null;

                if (result != null && result.Count > 0)
                {
                    response.Success = true;
                    response.Message = "Success";
                    response.Data = result;
                }
                else
                {
                    response.Success = true;
                    response.Message = "no data";
                }
                //write log
                var jsonOptionss = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptionss);
                General.WriteLogInTextFile(msg);
            }
            catch (Exception ex)
            {
                string msg = "Error :" + DateTime.Now + ">>>>" + ex.Message;
                General.WriteLogInTextFile(msg);

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        public async Task<ServiceResponse> CreatePayment(PaymentMethodModel model,string userId)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                if (model == null)
                {
                    response.Success = false;
                    response.Message = "Payment method data cannot be null.";
                    return response;
                }

                PaymentMethod? result = await _context.PaymentMethods
                        .Where(v => v.Active == true &&
                        v.Name == model.Name 
                        && v.PaymentMethodId == model.PaymentMethodId)
                        .FirstOrDefaultAsync();

                if (model.Discount > 100)
                {
                    response.Success = false;
                    response.Message = "Discount cannot exceed 100%";
                    return response;
                }
                if (result == null)
                { //throw new Exception("Payment is already existed!!!");
                    result = new PaymentMethod
                    {
                      
                        PaymentMethodId = Guid.NewGuid().ToString(),
                        CreatedOn = DateTime.Now,
                        CreatedBy = userId
                    };
                    _context.PaymentMethods.Add(result);
                }
                else
                {
                    result.ModifiedOn = DateTime.Now;
                    result.ModifiedBy = userId;
                }
                result.Name = model.Name;
                result.Discount = model.Discount;
                var res = await _context.SaveChangesAsync();

                if (res > 0)
                {
                    response.Success = true;
                    response.Message = "Success";
                }
                else
                {
                    response.Success = true;
                    response.Message = "failed";
                }
                //write log
                var jsonOptionss = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptionss);
                General.WriteLogInTextFile(msg);
            }
            catch (Exception ex)
            {
                string msg = "Error :" + DateTime.Now + ">>>>" + ex.Message;
                General.WriteLogInTextFile(msg);

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse> MakePayment(GiftCardPurchaseHistoryModel model,string userId)
        {
            ServiceResponse response = new ServiceResponse();
            //write log
            var jsonOptionss = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                var result = await _context.GiftCardPurchaseHistories
                        .Where(v => v.GiftCardId == model.GiftCardId
                        && v.PurchaseId == model.PurchaseId)
                        .FirstOrDefaultAsync();

               // if (result != null) throw new Exception("Gift Card is already existed!!!");

                var buyerList = await _context.GiftCardPurchaseHistories
                         .Where(v => v.GiftCardId == model.GiftCardId
                         && v.UserId == userId).ToListAsync();

                var receiverList = new List<GiftCardPurchaseHistory>();
                if (model.ReceiverID != null && model.ReceiverID != "")
                { 
                 receiverList = await _context.GiftCardPurchaseHistories
                    .Where(v => v.GiftCardId == model.GiftCardId
                    && v.ReceiverID == model.ReceiverID).ToListAsync();
                }

                var giftCardInfo = await _context.GiftCardDatas.Where(x => x.Active == true
                && x.GiftCardId == model.GiftCardId).FirstOrDefaultAsync();

                if (buyerList.Count >= giftCardInfo.MaxLimitToBuy)
                {
                    response.Success = false;
                    response.Message = $"Purchase limit exceeded. Maximum {giftCardInfo.MaxLimitToBuy} gift cards can be bought."; ;
                    return response;
                }
                if (receiverList.Count >= giftCardInfo.GiftPerUserLimit)
                {
                    response.Success = false;
                    response.Message = $"Gift limit exceeded. Maximum {giftCardInfo.GiftPerUserLimit} gift cards can be received per user.";
                    return response;
                }
                else
                {
                    var userDetail = model.ReceiverID != null && model.ReceiverID != "" ? 
                        await _context.Users.FirstOrDefaultAsync(x => x.UserId == model.ReceiverID && x.Active == true) : null;

                    if (result == null)
                    {
                        result = new GiftCardPurchaseHistory {
                            PurchaseId = Guid.NewGuid().ToString(),
                            CreatedBy = userId,
                            CreatedOn = DateTime.Now,
                            Active = true,
                            PromoCode = await GeneratePromoCode(),
                        };
                        _context.GiftCardPurchaseHistories.Add(result);
                    }
                    else
                    {
                        result.ModifiedBy = userId;
                        result.ModifiedOn = DateTime.Now;
                    }
                        //buy new one
                    result.UserId = userId;
                    result.IsGift = model.IsGift;
                    result.ReceiverID = model.ReceiverID;
                    result.GiftCardId = model.GiftCardId;
                    result.ReceiverName = userDetail != null ? userDetail.Username : "";
                    result.ReceiverPhoneNo = userDetail != null ? userDetail.PhoneNumber : "";
                    result.IsUsed = false;

                    result.QRPath = await GenerateQRCode(result.PromoCode);
                    //write log
                    string resdata = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(result, jsonOptionss);
                    General.WriteLogInTextFile(resdata);

                    var res = await _context.SaveChangesAsync();

                    if (res > 0)
                    {
                        response.Success = true;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Success = true;
                        response.Message = "failed";
                    }
                }
                   
                //write log
               
                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptionss);
                General.WriteLogInTextFile(msg);
            }
            catch (Exception ex)
            {
                string msg = "Error :" + DateTime.Now + ">>>>" + ex.Message;
                General.WriteLogInTextFile(msg);

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private async Task<string> GeneratePromoCode()
        {
            string promoCode;
            do
            {
                var random = new Random();
                var numbers = random.Next(100000, 999999).ToString();
                var letters = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                promoCode = $"{numbers}{letters}";
            } while (_context.GiftCardPurchaseHistories.Any(p => p.PromoCode == promoCode));

            return promoCode;
        }

        private async Task<string> GenerateQRCode(string data)
        {
            // Ensure the directory exists
            if (!Directory.Exists("wwwroot/qrcodes"))
            {
                Directory.CreateDirectory("wwwroot/qrcodes");
            }

            // Generate unique filename
            string fileName = $"qrcode_{Guid.NewGuid()}.png";
            string filePath = Path.Combine("wwwroot/qrcodes", fileName);

            // Generate QR code
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            // Save to file
            File.WriteAllBytes(filePath, qrCodeImage);

            return $"/qrcodes/{fileName}"; 
        }

       
        public async Task<ActionResult<VerifyResponse>> VerifyPromoCode(VerifyDto verifyDto)
        {
            var purchase = await _context.GiftCardPurchaseInformations
                .FirstOrDefaultAsync(p => p.PromoCode == verifyDto.PromoCode && !p.IsUsed);

            if (purchase == null)
                throw new Exception("Invalid or used promo code");

            return new VerifyResponse
            {
                Valid = true,
                Amount = purchase.Amount,
                GiftCardId = purchase.GiftCardId
            };
        }

        public async Task<ActionResult<ServiceResponse>> UsePromoCode(VerifyDto verifyDto)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                var purchase = await _context.GiftCardPurchaseHistories
                    .FirstOrDefaultAsync(p => p.PromoCode == verifyDto.PromoCode && !p.IsUsed);

                if (purchase == null)
                    throw new Exception("Invalid or used promo code");
                var giftData = await _context.GiftCardDatas.FirstOrDefaultAsync(x => x.GiftCardId == purchase.GiftCardId 
                && x.Active == true);

                if (giftData == null)
                {
                    response.Success = false;
                    response.Message = "Gift card not found";
                    return response;
                }

                if (giftData.ExpiryDate < DateTime.Now)
                {
                    response.Success = false;
                    response.Message = $"Gift card expired on {giftData.ExpiryDate:yyyy-MM-dd}";
                    return response;
                }

                purchase.IsUsed = true;
                purchase.UsedOn = DateTime.Now;
                var res = await _context.SaveChangesAsync();

                if (res > 0)
                {
                    response.Success = true;
                    response.Message = "Success";
                }
                else
                {
                    response.Success = true;
                    response.Message = "failed";
                }

                //write log
                var jsonOptionss = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptionss);
                General.WriteLogInTextFile(msg);
            }
            catch (Exception ex) {
                string msg = "Error :" + DateTime.Now + ">>>>" + ex.Message;
                General.WriteLogInTextFile(msg);

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }
    }
}
