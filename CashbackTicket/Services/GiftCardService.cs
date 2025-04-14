using AutoMapper;
using CashbackTicket.EFDBContext;
using CashbackTicket.Helper;
using CashbackTicket.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace CashbackTicket.Services
{

    public interface IGiftCardService
    {
        Task<ServiceDataResponse<List<GiftCardData>>> GetAllGiftCard();

        Task<ServiceDataResponse<GiftCardData>> GetGiftCardDetail(string giftCardId);
        Task<ServiceResponse> DeactivateGiftCard(string giftCardID);
        Task<ServiceResponse> CreateOrEditGiftCard(GiftCardDataModel model, string userId);
        Task<ServiceDataResponse<List<GiftCardDetailInformationView>>> UnUsedCashbacks(string userId);
        Task<ServiceDataResponse<List<GiftCardDetailInformationView>>> UsedCashbacks(string userId);
    }
    public class GiftCardService : IGiftCardService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public GiftCardService(AppDBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        #region get item
        public async Task<ServiceDataResponse<List<GiftCardData>>> GetAllGiftCard()
        {
            ServiceDataResponse<List<GiftCardData>> response = new ServiceDataResponse<List<GiftCardData>>();
            List<GiftCardData>? list = new List<GiftCardData>();
            try
            {
                var result = await _context.GiftCardDatas
                        .Where(v => v.Active == true )
                        .ToListAsync();

               // list = result != null ? _mapper.Map<List<GiftCardData>,List<GiftCardDataModel>>(result) : null;

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

        #region get item
        public async Task<ServiceDataResponse<GiftCardData>> GetGiftCardDetail(string giftCardId)
        {
            ServiceDataResponse<GiftCardData> response = new ServiceDataResponse<GiftCardData>();
            GiftCardData? model = new GiftCardData();
            try
            {
                var result = await _context.GiftCardDatas
                        .Where(v => v.GiftCardId == giftCardId && v.Active == true
                        )
                        .FirstOrDefaultAsync();
               // model = result != null ? _mapper.Map<GiftCardData, GiftCardDataModel>(result) : null;

                if (result != null)
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

        #region add item
        //public async Task<ServiceDataResponse<List<CartItem>>> AddItemToCartAsync(string userId, List<CartItemDTO> itemDto)
        //{
        //    ServiceDataResponse<List<CartItem>> response = new ServiceDataResponse<List<CartItem>>();
        //    try
        //    {
        //        var result = 0;
        //        var jsonOptions = new JsonSerializerOptions
        //        {
        //            WriteIndented = true
        //        };

        //        if (itemDto.Count > 0)
        //        {
        //            //write log

        //            string text = "info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptions);
        //            General.WriteLogInTextFile(text);
        //            foreach (var item in itemDto)
        //            {
        //                var product = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == item.ProductId
        //            && c.Active == true);

        //                if (product == null)
        //                    throw new Exception("Product not found");

        //                var existingItem = await _context.CartItems
        //                    .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == item.ProductId
        //                    && ci.Active == true);

        //                if (existingItem == null)
        //                {
        //                    existingItem = new CartItem
        //                    {
        //                        CartItemId = Guid.NewGuid().ToString(),
        //                        Active = true,
        //                        CreatedOn = DateTime.Now,
        //                        CreatedBy = userId
        //                    };
        //                    _context.CartItems.Add(existingItem);
        //                }
        //                else
        //                {
        //                    product.ModifiedBy = userId;
        //                    product.ModifiedOn = DateTime.Now;
        //                }

        //                existingItem.UserId = userId;
        //                existingItem.Qty = item.Qty;
        //                existingItem.ProductId = item.ProductId;
        //                existingItem.Price = product.Price;
        //                existingItem.Status = "InCart";
        //            }
        //            result = await _context.SaveChangesAsync();
        //        }
        //        if (result > 0)
        //        {
        //            response.Success = true;
        //            response.Message = "Success";
        //        }
        //        else
        //        {
        //            response.Success = false;
        //            response.Message = "Failed";
        //        }

        //        //write log
        //        string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptions);
        //        General.WriteLogInTextFile(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "Error :" + DateTime.Now + ">>>>" + ex.Message;
        //        General.WriteLogInTextFile(msg);
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        #endregion

        #region Remove 
        public async Task<ServiceResponse> DeactivateGiftCard(string giftCardID)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                var giftCardInfo = await _context.GiftCardDatas.Where(x => x.GiftCardId == giftCardID
                && x.Active == true).FirstOrDefaultAsync();

                if (giftCardInfo == null)
                    throw new Exception("Gift Card item not found");

                giftCardInfo.Active = false;
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
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptions);
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

        #region check out 
        public async Task<ServiceResponse> CreateOrEditGiftCard(GiftCardDataModel model,string userId)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                var result = 0;
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                if (model != null)
                {
                    //write log

                    string text = "info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(model, jsonOptions);
                    General.WriteLogInTextFile(text);

                    var giftCardInfo = await _context.GiftCardDatas.FirstOrDefaultAsync
                       (c => c.GiftCardId == model.GiftCardId
                       && c.Active == true);

                        if (giftCardInfo == null)
                        {
                            giftCardInfo = new GiftCardData
                            {
                                GiftCardId = Guid.NewGuid().ToString(),
                                Active = true,
                                CreatedOn = DateTime.Now,
                                CreatedBy = userId,
                                GiftCardNo = await GenerateUniqueGiftCardNumber()//Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()
                            };
                            _context.GiftCardDatas.Add(giftCardInfo);
                        }
                        else
                        {
                            giftCardInfo.ModifiedBy = userId;
                            giftCardInfo.ModifiedOn = DateTime.Now;
                        }
                        giftCardInfo.Title = model.Title;
                        giftCardInfo.Description = model.Description;
                        giftCardInfo.Quantity = model.Quantity;
                        giftCardInfo.PaymentMethodId = model.PaymentMethodId;
                        giftCardInfo.ExpiryDate = model.ExpiryDate;
                        giftCardInfo.Amount = model.Amount;
                        giftCardInfo.Discount = model.Discount;
                        giftCardInfo.GiftPerUserLimit = model.GiftPerUserLimit;
                        giftCardInfo.MaxLimitToBuy = model.MaxLimitToBuy;

                    result = await _context.SaveChangesAsync();
                }
                if (result > 0)
                {
                    response.Success = true;
                    response.Message = "Success";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed";
                }

                //write log
                string msg = "Info :" + DateTime.Now + " >>>> " + JsonSerializer.Serialize(response, jsonOptions);
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

        #region get item
        public async Task<ServiceDataResponse<List<GiftCardDetailInformationView>>> UnUsedCashbacks(string userId)
        {
            ServiceDataResponse<List<GiftCardDetailInformationView>> response = new ServiceDataResponse<List<GiftCardDetailInformationView>>();
            List<GiftCardDetailInformationView>? list = new List<GiftCardDetailInformationView>();
            try
            {
                var result = await _context.GiftCardPurchaseInformations
                        .Where(v => v.IsUsed == false && 
                        (v.UserId == userId || v.ReceiveUserID == userId))
                        .ToListAsync();

                //list = result != null ? _mapper.Map<List<GiftCardData>, List<GiftCardDataModel>>(result) : null;

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

        public async Task<ServiceDataResponse<List<GiftCardDetailInformationView>>> UsedCashbacks(string userId)
        {
            ServiceDataResponse<List<GiftCardDetailInformationView>> response = new ServiceDataResponse<List<GiftCardDetailInformationView>>();
            List<GiftCardDetailInformationView>? list = new List<GiftCardDetailInformationView>();
            try
            {
                var result = await _context.GiftCardPurchaseInformations
                        .Where(v => v.IsUsed == true && 
                        (v.UserId == userId || v.ReceiveUserID == userId))
                        .ToListAsync();

                //list = result != null ? _mapper.Map<List<GiftCardData>, List<GiftCardDataModel>>(result) : null;

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

        private async Task<string> GenerateUniqueGiftCardNumber()
        {
            string cardNo;
            do
            {
                cardNo = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
            } while (_context.GiftCardDatas.Any(g => g.GiftCardNo == cardNo));

            return cardNo;
        }
    }

}
