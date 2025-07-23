namespace ConsoleApp_ETA_eReceipts.Services
{
    public interface IReceiptSender
    {
        Task<string?> SendReceiptByIdAsync(int id, decimal? overrideAmount = null, bool forceCurrentDate = true);
    }

}
