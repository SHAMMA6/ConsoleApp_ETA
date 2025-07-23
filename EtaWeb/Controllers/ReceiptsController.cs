using ConsoleApp_ETA_eReceipts.Data;
using ConsoleApp_ETA_eReceipts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsoleApp_ETA_eReceipts.Mappers;

namespace EtaWeb.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly EtaDbContext _db;
        private readonly IReceiptSender _sender;

        public ReceiptsController(EtaDbContext db, IReceiptSender sender)
        {
            _db = db;
            _sender = sender;
        }

        // GET: Receipts
        public async Task<IActionResult> Index()
        {
            var receipts = await _db.Receipts.Include(r => r.Seller).ToListAsync();
            return View(receipts);
        }

        // GET: Receipts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var receipt = await _db.Receipts
                .Include(r => r.Seller)
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (receipt == null) return NotFound();
            return View(receipt);
        }

        // GET: Receipts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var receipt = await _db.Receipts.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
            if (receipt == null) return NotFound();
            return View(receipt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReceiptEntity receipt)
        {
            if (id != receipt.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(receipt);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(receipt);
        }

        // POST: Receipts/Submit/5
        [HttpPost]
        public async Task<IActionResult> Submit(int id)
        {
            var response = await _sender.SendReceiptByIdAsync(id);
            TempData["Response"] = response;
            return RedirectToAction("Details", new { id });
        }

        // GET: Receipts/ViewJson/5
        public async Task<IActionResult> ViewJson(int id)
        {
            var receipt = await _db.Receipts.Include(r => r.Items).Include(r => r.Seller).FirstOrDefaultAsync(r => r.Id == id);
            if (receipt == null) return NotFound();
            var json = System.Text.Json.JsonSerializer.Serialize(receipt.ToEtaReceipt(), new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            return Content(json, "application/json");
        }
    }
}
