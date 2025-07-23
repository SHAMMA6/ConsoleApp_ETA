using ConsoleApp_ETA_eReceipts;
using ConsoleApp_ETA_eReceipts.EtaDtos;
using ConsoleApp_ETA_eReceipts.Hasher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EtaWeb.Services
{
    public class ETAClient : IETAClient
    {
        private readonly HttpClient _http;
        private readonly ETAOptions _options;
        private readonly ILogger<ETAClient> _logger;

        public ETAClient(HttpClient http, IOptions<ETAOptions> options, ILogger<ETAClient> logger)
        {
            _http = http;
            _options = options.Value;
            _logger = logger;
        }

        // -----------------------------------------------------------------
        // NEW: Accept fully built Receipt and submit
        // -----------------------------------------------------------------
        public async Task<string?> SendReceiptToETAAsync(Receipt receipt)
        {
            string token = await GetETATokenAsync() ?? throw new Exception("Failed to get token");

            // Compute UUID from receipt JSON with blank uuid
            var headerNoUuid = receipt.Header with { Uuid = string.Empty };
            var receiptNoUuid = receipt with { Header = headerNoUuid };

            string receiptJson = JsonSerializer.Serialize(receiptNoUuid, JsonOptions);
            string uuid = ETAHashing.HashedSerializedData(receiptJson);

            // Inject uuid
            var receiptWithUuid = receipt with { Header = receipt.Header with { Uuid = uuid } };
            var submission = new ReceiptSubmission(new List<Receipt> { receiptWithUuid });

            string finalJson = JsonSerializer.Serialize(submission, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            // **طباعة JSON النهائي**
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==== JSON Payload to ETA ====");
            Console.ResetColor();
            Console.WriteLine(finalJson);
            Console.WriteLine("=============================");

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseApiUrl}/api/v1/receiptsubmissions")
            {
                Content = new StringContent(finalJson, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Sending receipt UUID={Uuid}", uuid);
            var resp = await _http.SendAsync(req);
            string body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                _logger.LogError("ETA error {Status}: {Body}", resp.StatusCode, body);
            else
                _logger.LogInformation("ETA OK: {Body}", body);

            return body;
        }

        // -----------------------------------------------------------------
        // Legacy quick test: create on-the-fly with single item
        // -----------------------------------------------------------------
        public async Task<string?> SendReceiptToETAAsync(decimal amount)
        {
            if (amount <= 0) { _logger.LogError("Invalid amount"); return null; }

            var dtIssued = DateTime.UtcNow.AddSeconds(-5).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            var header = new ReceiptHeader(dtIssued, $"E-{Random.Shared.Next(1000, 10000)}", "", "", "", "EGP");
            var documentType = new DocumentType("S", "1.2");
            var seller = new Seller("352247665", "Company Name", "0", new BranchAddress("EG", "Cairo Governorate", "city center", "16 street", "14BN", "74299"), "54545888", "102258", "9319");
            var buyer = new Buyer("F");
            var item = new ItemData("880609", "Samsung A02 32GB_LTE_BLACK_DS_SM-A022FZKDMEB_A022 _ A022_SM-", "EGS", "EG-697604748-222Ele", "EA", 1, amount, amount, amount, amount);
            var receipt = new Receipt(header, documentType, seller, buyer, new List<ItemData> { item }, amount, amount, amount, "V");
            return await SendReceiptToETAAsync(receipt);
        }

        // -----------------------------------------------------------------
        // Expose token if needed for diagnostics
        // -----------------------------------------------------------------
        public async Task<string?> GetTokenAsync() => await GetETATokenAsync();

        // Internal token fetch
        private async Task<string?> GetETATokenAsync()
        {
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseIdentityUrl}/connect/token")
            {
                Content = new FormUrlEncodedContent(form)
            };

            req.Headers.TryAddWithoutValidation("posserial", _options.PosSerial);
            req.Headers.TryAddWithoutValidation("pososversion", "android");
            req.Headers.TryAddWithoutValidation("presharedkey", "");

            var resp = await _http.SendAsync(req);
            var content = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Token error {Status}: {Content}", resp.StatusCode, content);
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("access_token", out var tokenEl))
                    return tokenEl.GetString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token parse error");
            }
            return null;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }
}
