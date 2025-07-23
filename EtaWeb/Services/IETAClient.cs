using ConsoleApp_ETA_eReceipts.EtaDtos;

namespace EtaWeb.Services
{
    public interface IETAClient
    {
        Task<string?> SendReceiptToETAAsync(decimal amount); 
        Task<string?> SendReceiptToETAAsync(Receipt receipt); 
        Task<string?> GetTokenAsync(); 
    }
}
