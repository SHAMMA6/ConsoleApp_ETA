namespace ConsoleApp_ETA_eReceipts
{
    public class ETAOptions
    {
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string PosSerial { get; set; } = "";
        public string BaseIdentityUrl { get; set; } = "https://id.preprod.eta.gov.eg";
        public string BaseApiUrl { get; set; } = "https://api.preprod.invoicing.eta.gov.eg";
    }

}
