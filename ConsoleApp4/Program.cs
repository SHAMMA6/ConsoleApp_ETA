using ConsoleApp_ETA_eReceipts;
using ConsoleApp_ETA_eReceipts.Data;
using ConsoleApp_ETA_eReceipts.Hasher;
using ConsoleApp_ETA_eReceipts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


var services = new ServiceCollection();

// Logging
services.AddLogging(builder => builder.AddConsole());

services.AddDbContext<EtaDbContext>(options =>
{
    options.UseSqlServer("Server=.;Database=Test_EReceipts_Consoul_App;User Id=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True");
});

services.Configure<ETAOptions>(opts =>
{
    opts.ClientId = "cd630b11-107d-4720-a398-e277ee6daefb";
    opts.ClientSecret = "7d4a2d75-ba8d-4baa-b1f1-4647f6544176";
    opts.PosSerial = "123456";
});

// HttpClient + ETAClient
services.AddHttpClient<IETAClient, ETAClient>();
services.AddScoped<IReceiptSender, ReceiptSender>();

var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();
var etaClient = provider.GetRequiredService<IETAClient>();

await EtaSeedData.EnsureSeedAsync(provider);

while (true)
{
    Console.WriteLine("\n====== ETA Console Menu ======");
    Console.WriteLine("1 - Send Receipt to ETA");
    Console.WriteLine("2 - Get Access Token");
    Console.WriteLine("3 - Test HashedSerializedData");
    Console.WriteLine("4 - Submit Receipt Id from DB");
    Console.WriteLine("0 - Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            await SendReceipt(etaClient, logger);
            break;
        case "2":
            await GetToken(etaClient);
            break;
        case "3":
            TestHash();
            break;
        case "4":
            Console.Write("Enter Receipt Id from DB: ");
            if (int.TryParse(Console.ReadLine(), out var rid))
            {
                var sender = provider.GetRequiredService<IReceiptSender>();
                var resp = await sender.SendReceiptByIdAsync(rid);
                Console.WriteLine("ETA Response:\n" + resp);
            }
            else
            {
                Console.WriteLine("Invalid Id");
            }
            break;
        case "0":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid choice, try again!");
            break;
    }
}

static async Task SendReceipt(IETAClient etaClient, ILogger logger)
{
    Console.Write("Enter amount: ");
    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        logger.LogInformation("Sending receipt for {Amount}", amount);
        var response = await etaClient.SendReceiptToETAAsync(amount);
        Console.WriteLine("ETA Response:\n" + response);
    }
    else
    {
        Console.WriteLine("Invalid amount.");
    }
}

static async Task GetToken(IETAClient etaClient)
{
    try
    {
        // Using Reflection to call GetETATokenAsync from ETAClient
        var tokenMethod = etaClient.GetType().GetMethod("GetETATokenAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (tokenMethod != null)
        {
            var task = (Task<string?>)tokenMethod.Invoke(etaClient, null)!;
            var token = await task;
            Console.WriteLine("Access Token: " + token);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error getting token: " + ex.Message);
    }
}

static void TestHash()
{
    Console.WriteLine("Enter JSON to hash:");
    string? json = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(json))
    {
        string hash = ETAHashing.HashedSerializedData(json);
        Console.WriteLine("Hash (SHA256): " + hash);
    }
    else
    {
        Console.WriteLine("Invalid JSON input.");
    }
}




