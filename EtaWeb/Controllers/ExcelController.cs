using Microsoft.AspNetCore.Mvc;

namespace EtaWeb.Controllers
{
    public class ExcelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
