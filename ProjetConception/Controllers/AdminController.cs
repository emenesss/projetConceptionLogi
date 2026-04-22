using Microsoft.AspNetCore.Mvc;
using ProjetConception.Models;
using ProjetConception.DAO;

namespace ProjetConception.Controllers
{
    public class AdminController : Controller
    {
        ClientDao clientDao = new ClientDao();
        UserDao userDao = new UserDao();
        
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");
            
            List<User> users = userDao.SelectAllUsers();

            return View(users);
        }

        [HttpGet]
        public IActionResult GrantAccess()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");
            

            ViewBag.Users = userDao.SelectAllUsers();
            ViewBag.Clients = clientDao.SelectAllClients();
            return View();
        }

        [HttpPost]
        public IActionResult GrantAccess(int userId, int clientId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            User? user = userDao.SelectUserById(userId);
            Client? client = clientDao.SelectClientById(clientId);

            bool success = false;

            if (user != null && client != null)
            {
                if (!user.AccessibleClientIds.Contains(clientId))
                {
                    user.AccessibleClientIds.Add(clientId);
                    success = userDao.UpdateAccessibleClientIds(userId, user.AccessibleClientIds) > 0;
                }
            }

            ViewBag.Users = userDao.SelectAllUsers();
            ViewBag.Clients = clientDao.SelectAllClients();
            ViewBag.Message = success
                ? "Accès attribué avec succès."
                : "Impossible d'attribuer l'accès.";

            return View();
        }
    }
}