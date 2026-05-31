using Microsoft.AspNetCore.Mvc;

namespace RecetasHub.Controllers
{
    public class AdminController : Controller
    {

        public IActionResult FuentesRecetas()
        {
            return View();
        }

        public IActionResult GestionUsuarios()
        {
            return View();
        }

        public IActionResult Secrets()
        {
            return View();
        }
    }
}
