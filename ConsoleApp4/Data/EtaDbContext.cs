using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_ETA_eReceipts.Data
{
    public class EtaDbContext : DbContext
    {
        public EtaDbContext(DbContextOptions<EtaDbContext> options) : base(options) { }

        public DbSet<ReceiptEntity> Receipts => Set<ReceiptEntity>();
        public DbSet<SellerEntity> Sellers => Set<SellerEntity>();
        public DbSet<ReceiptItemEntity> ReceiptItems => Set<ReceiptItemEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReceiptItemEntity>()
                .HasOne(i => i.Receipt)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.ReceiptEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceiptEntity>()
                .HasOne(r => r.Seller)
                .WithMany(s => s.Receipts)
                .HasForeignKey(r => r.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
