using ConsoleApp_ETA_eReceipts;
using ConsoleApp_ETA_eReceipts.Data;
using ConsoleApp_ETA_eReceipts.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=.;Database=Test_EReceipts_Consoul_App;User Id=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True";

builder.Services.AddDbContext<EtaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<ETAOptions>(opts =>
{
    opts.ClientId = "cd630b11-107d-4720-a398-e277ee6daefb";
    opts.ClientSecret = "7d4a2d75-ba8d-4baa-b1f1-4647f6544176";
    opts.PosSerial = "123456";
});


builder.Services.AddHttpClient<IETAClient, ETAClient>();
builder.Services.AddScoped<IReceiptSender, ReceiptSender>();

var provider = builder.Services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();
var etaClient = provider.GetRequiredService<IETAClient>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Receipts}/{action=Index}/{id?}");

app.Run();
