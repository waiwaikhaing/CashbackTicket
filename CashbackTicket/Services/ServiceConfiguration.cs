namespace CashbackTicket.Services
{
    public class ServiceConfiguration
    {
        public void AddServices(IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGiftCardService, GiftCardService>();
            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
