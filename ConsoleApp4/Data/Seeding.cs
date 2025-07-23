using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp_ETA_eReceipts.Data
{
    public static class EtaSeedData
    {

        public static async Task EnsureSeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EtaDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("EtaSeedData");

            await db.Database.MigrateAsync();

            var seller = await db.Sellers.FirstOrDefaultAsync();
            if (seller == null)
            {
                seller = new SellerEntity
                {
                    Rin = "352247665",
                    CompanyTradeName = "Your Company Trade Name",
                    BranchCode = "0",
                    Country = "EG",
                    Governate = "Cairo",
                    RegionCity = "Nasr City",
                    Street = "123 Example St",
                    BuildingNumber = "B1",
                    PostalCode = "11765",
                    DeviceSerialNumber = "DEV-001",
                    SyndicateLicenseNumber = "LIC-001",
                    ActivityCode = "6201"
                };
                db.Sellers.Add(seller);
                await db.SaveChangesAsync();
            }


            // Helper local func to build receipts quickly
            ReceiptEntity NewReceipt(string number, decimal amount, DateTime issuedUtc) => new()
            {
                ReceiptNumber = number,
                DateTimeIssuedUtc = issuedUtc.ToString(),
                Currency = "EGP",
                SellerId = seller.Id,
                BuyerType = "F",
                ReceiptType = "S",
                TypeVersion = "1.2",
                TotalSales = amount,
                NetAmount = amount,
                TotalAmount = amount,
                PaymentMethod = "V",
                Status = ReceiptStatus.Draft,
                CreatedUtc = DateTime.UtcNow
            };

            // -----------------------------------
            // Receipt 1: Single mobile handset
            // -----------------------------------
            var r1 = NewReceipt("E-1001", 75, new DateTime(2025, 07, 15, 10, 00, 00, DateTimeKind.Utc));
            r1.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r1.Id,
                InternalCode = "MOB-001",
                Description = "Samsung A02 32GB Black",
                ItemType = "EGS",
                ItemCode = "EG-352247665-MOB001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 75,
                NetSale = 75,
                TotalSale = 75,
                Total = 75
            });

            // -----------------------------------
            // Receipt 2: Laptop + Mouse bundle
            // -----------------------------------
            var r2 = NewReceipt("E-1002", 153, new DateTime(2025, 07, 16, 09, 30, 00, DateTimeKind.Utc));
            r2.Items.Add(new ReceiptItemEntity
            {

                ReceiptEntityId = r2.Id,
                InternalCode = "LTP-001",
                Description = "Laptop 15\" i5 8GB",
                ItemType = "EGS",
                ItemCode = "EG-352247665-LTP001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 15,
                NetSale = 15,
                TotalSale = 10,
                Total = 15
            });
            r2.Items.Add(new ReceiptItemEntity
            {

                ReceiptEntityId = r2.Id,
                InternalCode = "MOU-001",
                Description = "USB Optical Mouse",
                ItemType = "EGS",
                ItemCode = "EG-352247665-MOU001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 3,
                NetSale = 3,
                TotalSale = 3,
                Total = 3
            });

            // -----------------------------------
            // Receipt 3: 3 different accessories
            // -----------------------------------
            var r3 = NewReceipt("E-1003", 92, new DateTime(2025, 07, 17, 14, 15, 00, DateTimeKind.Utc));
            r3.Items.Add(new ReceiptItemEntity
            {

                ReceiptEntityId = r3.Id,
                InternalCode = "CAB-USB",
                Description = "USB-C Cable 1m",
                ItemType = "EGS",
                ItemCode = "EG-352247665-CABUSB",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 1,
                NetSale = 1,
                TotalSale = 1,
                Total = 1
            });
            r3.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r3.Id,
                InternalCode = "SCR-001",
                Description = "Screen Protector",
                ItemType = "EGS",
                ItemCode = "EG-352247665-SCR001",
                UnitType = "EA",
                Quantity = 2,
                UnitPrice = 8,
                NetSale = 1,
                TotalSale = 16,
                Total = 16
            });
            r3.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r3.Id,
                InternalCode = "HDP-001",
                Description = "Headphones Basic",
                ItemType = "EGS",
                ItemCode = "EG-352247665-HDP001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 6,
                NetSale = 6,
                TotalSale = 6,
                Total = 6
            });
            // -----------------------------------
            // Receipt 4: Tablet
            // -----------------------------------
            var r4 = NewReceipt("E-1004", 56, new DateTime(2025, 07, 18, 11, 45, 00, DateTimeKind.Utc));
            r4.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r4.Id,
                InternalCode = "TAB-001",
                Description = "Samsung Tab A7 10.4\" 64GB",
                ItemType = "EGS",
                ItemCode = "EG-352247665-TAB001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 5600,
                NetSale = 5600,
                TotalSale = 5600,
                Total = 5600
            });

            // -----------------------------------
            // Receipt 5: Gaming Bundle
            // -----------------------------------
            var r5 = NewReceipt("E-1005", 78, new DateTime(2025, 07, 18, 15, 30, 00, DateTimeKind.Utc));
            r5.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r5.Id,
                InternalCode = "PS5-001",
                Description = "PlayStation 5 Console",
                ItemType = "EGS",
                ItemCode = "EG-352247665-PS5001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 7500,
                NetSale = 7500,
                TotalSale = 7500,
                Total = 7500
            });
            r5.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r5.Id,
                InternalCode = "PAD-001",
                Description = "PS5 DualSense Controller",
                ItemType = "EGS",
                ItemCode = "EG-352247665-PAD001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 300,
                NetSale = 300,
                TotalSale = 300,
                Total = 300
            });

            // -----------------------------------
            // Receipt 6: Smartwatch
            // -----------------------------------
            var r6 = NewReceipt("E-1006", 25, new DateTime(2025, 07, 19, 09, 15, 00, DateTimeKind.Utc));
            r6.Items.Add(new ReceiptItemEntity
            {
                ReceiptEntityId = r6.Id,
                InternalCode = "WTC-001",
                Description = "Huawei Watch GT 4",
                ItemType = "EGS",
                ItemCode = "EG-352247665-WTC001",
                UnitType = "EA",
                Quantity = 1,
                UnitPrice = 2500,
                NetSale = 2500,
                TotalSale = 2500,
                Total = 2500
            });

            // add receipts
            db.Receipts.AddRange(r1, r2, r3, r4, r5, r6);
            await db.SaveChangesAsync();

            logger.LogInformation("Seed complete: 1 seller + 3 receipts (rin=352247665).");
        }
    }

}
