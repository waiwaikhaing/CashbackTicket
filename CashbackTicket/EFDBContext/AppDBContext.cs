using CashbackTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace CashbackTicket.EFDBContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDBContext()
        {
        }

        public DbSet<UserData> Users { get; set; }
        public DbSet<GiftCardData> GiftCardDatas { get; set; }
        public DbSet<GiftCardPurchaseHistory> GiftCardPurchaseHistories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<GiftCardDetailInformationView> GiftCardPurchaseInformations{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GiftCardDetailInformationView>()
                .HasNoKey()
                .ToView("GiftCardDetailInformationView");

            modelBuilder.Entity<GiftCardData>()
               .HasKey(c => c.GiftCardId);

            modelBuilder.Entity<UserData>()
               .HasKey(c => c.UserId);

            modelBuilder.Entity<GiftCardPurchaseHistory>()
             .HasKey(c => c.PurchaseId);

            modelBuilder.Entity<PaymentMethod>()
            .HasKey(c => c.PaymentMethodId);
        }

    }
}
