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
        
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            return View();
        }

        
        [HttpPost]
        public IActionResult CreateUser(int id, string name, string username, string password, Role role)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Tous les champs sont obligatoires.";
                return View();
            }

            User user = new User(id, name, username, password, role);

            userDao.InsertUser(user);

            TempData["Message"] = "Utilisateur créé avec succès.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            userDao.DeleteUser(id);

            TempData["Message"] = "Utilisateur supprimé.";
            return RedirectToAction("Users");
        }

        
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            var user = userDao.SelectUserById(id);

            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        
        [HttpPost]
        public IActionResult EditUser(int id, string name, string username, string password, Role role)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Client");

            userDao.UpdateUserName(id, name);
            userDao.UpdateUsername(id, username);

            if (!string.IsNullOrWhiteSpace(password))
                userDao.UpdatePassword(id, password);

            userDao.UpdateRole(id, role);

            TempData["Message"] = "Utilisateur modifié.";
            return RedirectToAction("Users");
        }
    }
}