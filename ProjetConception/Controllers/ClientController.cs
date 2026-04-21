using Microsoft.AspNetCore.Mvc;
using ProjetConception.Models;
using ProjetConception.DAO;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjetConception.Controllers
{
    
    /// 
    /// Contrôleur responsable de l'affichage et de la gestion des clients.
    /// L'administrateur peut voir tous les clients, tandis que l'utilisateur final
    /// ne voit que les clients auxquels il a accès.
    /// 
    public class ClientController : Controller
    {
        
        /// 
        /// Affiche la page principale des clients.
        /// Le contenu affiché varie selon le rôle de l'utilisateur connecté.
        /// 
        public IActionResult Index()
        {
            int? idUtilisateur = HttpContext.Session.GetInt32("UserId");
            string? roleUtilisateur = HttpContext.Session.GetString("Role");

            if (idUtilisateur == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User? utilisateurCourant = DataStore.ISP.Users
                .FirstOrDefault(u => u.Id == idUtilisateur);

            if (utilisateurCourant == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var listeClients = roleUtilisateur == "Admin"
                ? DataStore.ISP.Clients
                : DataStore.ISP.Clients
                    .Where(c => utilisateurCourant.AccessibleClientIds.Contains(c.Id))
                    .ToList();

            ViewBag.PageTitle = roleUtilisateur == "Admin"
                ? "Tableau de bord administrateur"
                : "Mes clients";

            ViewBag.IsAdmin = roleUtilisateur == "Admin";

            return View(listeClients);
        }

        /// 
        /// Affiche le formulaire de création d'un client.
        /// Réservé à l'administrateur.
        /// 
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        /// 
        /// Traite la création d'un nouveau client.
        /// Réservé à l'administrateur.
        /// 
        [HttpPost]
        public IActionResult Create(int id, string name)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Le nom du client est obligatoire.";
                return View();
            }

            bool identifiantDejaUtilise = DataStore.ISP.Clients.Any(c => c.Id == id);

            if (identifiantDejaUtilise)
            {
                ViewBag.Error = "Cet identifiant de client est déjà utilisé.";
                return View();
            }

            Client nouveauClient = new Client(id, name);
            DataStore.ISP.Clients.Add(nouveauClient);

            return RedirectToAction("Index");
        }

        /// 
        /// Supprime un client existant.
        /// Réservé à l'administrateur.
        /// 
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            Client? clientASupprimer = DataStore.ISP.Clients
                .FirstOrDefault(c => c.Id == id);

            if (clientASupprimer != null)
            {
                DataStore.ISP.Clients.Remove(clientASupprimer);
            }

            return RedirectToAction("Index");
        }

        /// 
        /// Affiche le détail d'un client et la liste de ses décodeurs.
        /// L'accès dépend du rôle et des permissions de l'utilisateur.
        /// 
        public IActionResult Details(int id)
        {
            int? idUtilisateur = HttpContext.Session.GetInt32("UserId");
            string? roleUtilisateur = HttpContext.Session.GetString("Role");

            if (idUtilisateur == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User? utilisateurCourant = DataStore.ISP.Users
                .FirstOrDefault(u => u.Id == idUtilisateur);

            Client? clientSelectionne = DataStore.ISP.Clients
                .FirstOrDefault(c => c.Id == id);

            if (utilisateurCourant == null || clientSelectionne == null)
            {
                return RedirectToAction("Index");
            }

            bool accesAutorise = roleUtilisateur == "Admin" ||
                                 utilisateurCourant.AccessibleClientIds.Contains(clientSelectionne.Id);

            if (!accesAutorise)
            {
                return RedirectToAction("Index");
            }

            ViewBag.PageTitle = roleUtilisateur == "Admin"
                ? $"Gestion du client : {clientSelectionne.Name}"
                : $"Client : {clientSelectionne.Name}";

            ViewBag.IsAdmin = roleUtilisateur == "Admin";

            return View(clientSelectionne);
        }
    }
}