using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp_ETA_eReceipts.Data
{
    public class ReceiptEntity
    {
        [Key] public int Id { get; set; }

        [MaxLength(64)] public string ReceiptNumber { get; set; } = $"E-{Random.Shared.Next(1000, 10000)}";
        public string DateTimeIssuedUtc { get; set; } = DateTime.UtcNow.AddSeconds(-5).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        [MaxLength(3)] public string Currency { get; set; } = EtaDefaults.Currency;

        [MaxLength(128)] public string? PreviousUuid { get; set; }
        [MaxLength(128)] public string? ReferenceOldUuid { get; set; }

        public int? SellerId { get; set; }
        public SellerEntity? Seller { get; set; }

        [MaxLength(8)] public string BuyerType { get; set; } = EtaDefaults.BuyerType;
        [MaxLength(4)] public string ReceiptType { get; set; } = EtaDefaults.ReceiptType;
        [MaxLength(8)] public string TypeVersion { get; set; } = EtaDefaults.TypeVersion;

        [Column(TypeName = "decimal(18,2)")] public decimal TotalSales { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NetAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalAmount { get; set; }

        [MaxLength(4)] public string PaymentMethod { get; set; } = EtaDefaults.PaymentMethod;

        public List<ReceiptItemEntity> Items { get; set; } = new();

        [MaxLength(128)] public string? EtaUuid { get; set; }
        [MaxLength(32)] public string Status { get; set; } = ReceiptStatus.Draft;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? SentUtc { get; set; }
        public DateTime? LastUpdatedUtc { get; set; }
        public string? EtaResponseRaw { get; set; }
    }

    public class SellerEntity
    {
        [Key] public int Id { get; set; }

        [MaxLength(32)] public string Rin { get; set; } = EtaDefaults.Rin;
        [MaxLength(256)] public string CompanyTradeName { get; set; } = EtaDefaults.CompanyTradeName;
        [MaxLength(16)] public string BranchCode { get; set; } = EtaDefaults.BranchCode;

        [MaxLength(4)] public string Country { get; set; } = EtaDefaults.Country;
        [MaxLength(128)] public string Governate { get; set; } = EtaDefaults.Governate;
        [MaxLength(128)] public string RegionCity { get; set; } = EtaDefaults.RegionCity;
        [MaxLength(128)] public string Street { get; set; } = EtaDefaults.Street;
        [MaxLength(32)] public string BuildingNumber { get; set; } = EtaDefaults.BuildingNumber;
        [MaxLength(16)] public string PostalCode { get; set; } = EtaDefaults.PostalCode;

        [MaxLength(64)] public string DeviceSerialNumber { get; set; } = EtaDefaults.DeviceSerialNumber;
        [MaxLength(64)] public string SyndicateLicenseNumber { get; set; } = EtaDefaults.SyndicateLicenseNumber;
        [MaxLength(32)] public string ActivityCode { get; set; } = EtaDefaults.ActivityCode;

        public List<ReceiptEntity> Receipts { get; set; } = new();
    }

    public class ReceiptItemEntity
    {
        [Key] public int Id { get; set; }
        public int ReceiptEntityId { get; set; } // FK name explicit to avoid confusion
        public ReceiptEntity Receipt { get; set; } = null!;

        [MaxLength(64)] public string InternalCode { get; set; } = EtaDefaults.DefaultInternalCode;
        [MaxLength(512)] public string Description { get; set; } = EtaDefaults.DefaultDescription;
        [MaxLength(16)] public string ItemType { get; set; } = EtaDefaults.DefaultItemType;
        [MaxLength(128)] public string ItemCode { get; set; } = EtaDefaults.DefaultItemCode;
        [MaxLength(16)] public string UnitType { get; set; } = EtaDefaults.DefaultUnitType;

        [Column(TypeName = "decimal(18,2)")] public decimal Quantity { get; set; } = 1m;
        [Column(TypeName = "decimal(18,2)")] public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NetSale { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalSale { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }
    }

    public static class ReceiptStatus
    {
        public const string Draft = "Draft";
        public const string Pending = "Pending";
        public const string Sent = "Sent";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
        public const string Error = "Error";
    }

}
