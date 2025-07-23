namespace EtaWeb.Services
{
    public interface IReceiptSender
    {
        Task<string?> SendReceiptByIdAsync(int id, decimal? overrideAmount = null, bool forceCurrentDate = true);
    }

}
