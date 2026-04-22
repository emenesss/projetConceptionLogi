using Microsoft.AspNetCore.Mvc;
using ProjetConception.Models;
using System.Linq;
using ProjetConception.DTO;
using ProjetConception.DAO;

namespace ProjetConception.Controllers
{
    public class AccountController : Controller
    {
        UserDao userDao = new UserDao();
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            User? user = userDao.SelectUserByUsernameAndPassword(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                ViewBag.Error = "Login invalide";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("Name", user.Name);

            return RedirectToAction("Index", "Client");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}