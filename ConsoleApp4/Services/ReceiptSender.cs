using ConsoleApp_ETA_eReceipts.Data;
using ConsoleApp_ETA_eReceipts.Hasher;
using ConsoleApp_ETA_eReceipts.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ConsoleApp_ETA_eReceipts.Services
{
    public class ReceiptSender : IReceiptSender
    {
        private readonly EtaDbContext _db;
        private readonly IETAClient _etaClient;
        private readonly ILogger<ReceiptSender> _logger;

        public ReceiptSender(EtaDbContext db, IETAClient etaClient, ILogger<ReceiptSender> logger)
        {
            _db = db;
            _etaClient = etaClient;
            _logger = logger;
        }

        public async Task<string?> SendReceiptByIdAsync(int id, decimal? overrideAmount = null, bool forceCurrentDate = true)
        {
            var entity = await _db.Receipts
                .Include(r => r.Seller)
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity is null)
            {
                _logger.LogWarning("Receipt id {Id} not found", id);
                return null;
            }

            var receiptDto = entity.ToEtaReceipt(overrideAmount);

            if (forceCurrentDate)
            {
                var nowIssued = DateTime.UtcNow.AddSeconds(-5)
                    .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
                receiptDto = receiptDto with { Header = receiptDto.Header with { DateTimeIssued = nowIssued } };
            }

            string? response = await _etaClient.SendReceiptToETAAsync(receiptDto);

            var noUuid = receiptDto with { Header = receiptDto.Header with { Uuid = string.Empty } };
            var jsonForHash = JsonSerializer.Serialize(noUuid, _camelNoIndent);
            var uuid = ETAHashing.HashedSerializedData(jsonForHash);

            entity.EtaUuid = uuid;
            entity.Status = ReceiptStatus.Sent;
            entity.SentUtc = DateTime.UtcNow;
            entity.EtaResponseRaw = response;
            await _db.SaveChangesAsync();
            return response;
        }

        static readonly JsonSerializerOptions _camelNoIndent = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

    }
}
