using System.Text.Json.Serialization;

namespace ConsoleApp_ETA_eReceipts.EtaDtos
{
    public record ReceiptSubmission(
    [property: JsonPropertyName("receipts")] List<Receipt> Receipts
);

    public record Receipt(
        [property: JsonPropertyName("header")] ReceiptHeader Header,
        [property: JsonPropertyName("documentType")] DocumentType DocumentType,
        [property: JsonPropertyName("seller")] Seller Seller,
        [property: JsonPropertyName("buyer")] Buyer Buyer,
        [property: JsonPropertyName("itemData")] List<ItemData> ItemData,
        [property: JsonPropertyName("totalSales")] decimal TotalSales,
        [property: JsonPropertyName("netAmount")] decimal NetAmount,
        [property: JsonPropertyName("totalAmount")] decimal TotalAmount,
        [property: JsonPropertyName("paymentMethod")] string PaymentMethod
    );

    public record ReceiptHeader(
        [property: JsonPropertyName("dateTimeIssued")] string DateTimeIssued,
        [property: JsonPropertyName("receiptNumber")] string ReceiptNumber,
        [property: JsonPropertyName("uuid")] string Uuid,
        [property: JsonPropertyName("previousUUID")] string PreviousUuid,
        [property: JsonPropertyName("referenceOldUUID")] string ReferenceOldUuid,
        [property: JsonPropertyName("currency")] string Currency
    );

    public record DocumentType(
        [property: JsonPropertyName("receiptType")] string ReceiptType,
        [property: JsonPropertyName("typeVersion")] string TypeVersion
    );

    public record Seller(
        [property: JsonPropertyName("rin")] string Rin,
        [property: JsonPropertyName("companyTradeName")] string CompanyTradeName,
        [property: JsonPropertyName("branchCode")] string BranchCode,
        [property: JsonPropertyName("branchAddress")] BranchAddress BranchAddress,
        [property: JsonPropertyName("deviceSerialNumber")] string DeviceSerialNumber,
        [property: JsonPropertyName("syndicateLicenseNumber")] string SyndicateLicenseNumber,
        [property: JsonPropertyName("activityCode")] string ActivityCode
    );

    public record BranchAddress(
        [property: JsonPropertyName("country")] string Country,
        [property: JsonPropertyName("governate")] string Governate,
        [property: JsonPropertyName("regionCity")] string RegionCity,
        [property: JsonPropertyName("street")] string Street,
        [property: JsonPropertyName("buildingNumber")] string BuildingNumber,
        [property: JsonPropertyName("postalCode")] string PostalCode
    );

    public record Buyer(
        [property: JsonPropertyName("type")] string Type
    );

    public record ItemData(
        [property: JsonPropertyName("internalCode")] string InternalCode,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("itemType")] string ItemType,
        [property: JsonPropertyName("itemCode")] string ItemCode,
        [property: JsonPropertyName("unitType")] string UnitType,
        [property: JsonPropertyName("quantity")] decimal Quantity,
        [property: JsonPropertyName("unitPrice")] decimal UnitPrice,
        [property: JsonPropertyName("netSale")] decimal NetSale,
        [property: JsonPropertyName("totalSale")] decimal TotalSale,
        [property: JsonPropertyName("total")] decimal Total
    );
}
