using Microsoft.AspNetCore.Mvc;
using ProjetConception.Models;

namespace ProjetConception.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            return View(DataStore.ISP.Users);
        }

        [HttpGet]
        public IActionResult GrantAccess()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            ViewBag.Users = DataStore.ISP.Users;
            ViewBag.Clients = DataStore.ISP.Clients;
            return View();
        }

        [HttpPost]
        public IActionResult GrantAccess(int userId, int clientId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            bool success = DataStore.ISP.GrantUserAccessToClient(userId, clientId);

            ViewBag.Users = DataStore.ISP.Users;
            ViewBag.Clients = DataStore.ISP.Clients;
            ViewBag.Message = success
                ? "Accès attribué avec succès."
                : "Impossible d'attribuer l'accès.";

            return View();
        }
    }
}