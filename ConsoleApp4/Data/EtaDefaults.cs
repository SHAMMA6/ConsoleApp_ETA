using ConsoleApp_ETA_eReceipts.EtaDtos;

namespace ConsoleApp_ETA_eReceipts.Data
{
    public static class EtaDefaults
    {
        // --- Seller defaults (RIN YOU REQUIRED) ---
        public const string Rin = "352247665";
        public const string CompanyTradeName = "Company Name";
        public const string BranchCode = "0";
        public const string Country = "EG";
        public const string Governate = "Cairo Governorate";
        public const string RegionCity = "city center";
        public const string Street = "16 street";
        public const string BuildingNumber = "14BN";
        public const string PostalCode = "74299";
        public const string DeviceSerialNumber = "54545888";
        public const string SyndicateLicenseNumber = "102258";
        public const string ActivityCode = "9319";

        // --- Buyer ---
        public const string BuyerType = "F";

        // --- DocumentType ---
        public const string ReceiptType = "S";
        public const string TypeVersion = "1.2";

        // --- Currency ---
        public const string Currency = "EGP";

        // --- Payment ---
        public const string PaymentMethod = "V";

        // --- Default Item ---
        public const string DefaultInternalCode = "880609";
        public const string DefaultDescription = "Samsung";
        public const string DefaultItemType = "EGS";
        public const string DefaultItemCode = "EG-697604748-222Ele"; // sample; change if ETA rejects
        public const string DefaultUnitType = "EA";

        // Build a fallback Receipt *DTO* with the given amount.
        public static ConsoleApp_ETA_eReceipts.EtaDtos.Receipt BuildFallbackReceipt(decimal amount, string? receiptNumber = null, DateTime? issuedUtc = null)
        {
            var dtIssued = (issuedUtc ?? DateTime.UtcNow.AddSeconds(-5))
                .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            receiptNumber ??= $"E-{Random.Shared.Next(1000, 10000)}";

            var header = new    ReceiptHeader(
                DateTimeIssued: dtIssued,
                ReceiptNumber: receiptNumber,
                Uuid: string.Empty,
                PreviousUuid: string.Empty,
                ReferenceOldUuid: string.Empty,
                Currency: Currency
            );

            var documentType = new DocumentType(ReceiptType, TypeVersion);

            var branchAddr = new BranchAddress(
                Country, Governate, RegionCity, Street, BuildingNumber, PostalCode);

            var seller = new Seller(
                Rin, CompanyTradeName, BranchCode, branchAddr,
                DeviceSerialNumber, SyndicateLicenseNumber, ActivityCode);

            var buyer = new Buyer(BuyerType);

            var item = new ItemData(
                DefaultInternalCode, DefaultDescription, DefaultItemType, DefaultItemCode, DefaultUnitType,
                Quantity: 1m,
                UnitPrice: amount,
                NetSale: amount,
                TotalSale: amount,
                Total: amount);

            return new Receipt(
                Header: header,
                DocumentType: documentType,
                Seller: seller,
                Buyer: buyer,
                ItemData: new List<ItemData> { item },
                TotalSales: amount,
                NetAmount: amount,
                TotalAmount: amount,
                PaymentMethod: PaymentMethod
            );
        }
    }
}
