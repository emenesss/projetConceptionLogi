using Microsoft.AspNetCore.Mvc;
using ProjetConception.Models;
using System.Linq;
using System.Threading.Tasks;
using ProjetConception.DAO;
using System.Text.Json;

namespace ProjetConception.Controllers
{
    /// 
    /// Contrôleur responsable de la gestion des décodeurs :
    /// création, suppression, consultation et actions via l'API du simulateur.
    /// 
    public class DecoderController : Controller
    {
        // Service utilisé pour communiquer avec l'API du professeur
        private readonly DecoderApiService _serviceApiDecodeur = new DecoderApiService();

        /// 
        /// Affiche le formulaire permettant d'ajouter un décodeur à un client.
        /// Réservé à l'administrateur.
        /// 
        [HttpGet]
        public IActionResult Create(int clientId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Client");
            }

            ViewBag.ClientId = clientId;
            return View();
        }

        /// 
        /// Traite l'ajout d'un nouveau décodeur à un client.
        /// Réservé à l'administrateur.
        /// 
        [HttpPost]
        public IActionResult Create(int clientId, int id, string serialNumber, string address)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Client");
            }

            ClientDao clientDao = new ClientDao();
            DecodeurDao decodeurDao = new DecodeurDao();

            Client? clientSelectionne = clientDao.SelectClientById(clientId);

            if (clientSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            if (string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(address))
            {
                ViewBag.Error = "Le numéro de série et l'adresse IP sont obligatoires.";
                ViewBag.ClientId = clientId;
                return View();
            }
            

            if (decodeurDao.DecodeurIdExists(id))
            {
                ViewBag.Error = "Cet identifiant de décodeur est déjà utilisé.";
                ViewBag.ClientId = clientId;
                return View();
            }

            Decodeur nouveauDecodeur = new Decodeur(id, serialNumber, clientId, address);
            decodeurDao.InsertDecodeur(nouveauDecodeur);
            clientSelectionne.AddDecoder(nouveauDecodeur);
            clientDao.UpdateClientDecodeurs(clientId, clientSelectionne.Decoders);

            return RedirectToAction("Details", "Client", new { id = clientId });
        }

        /// 
        /// Supprime un décodeur associé à un client.
        /// Réservé à l'administrateur.
        /// 
        public IActionResult Delete(int clientId, int decoderId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Client");
            }

            ClientDao clientDao = new ClientDao();

            Client? clientSelectionne = clientDao.SelectClientById(clientId);

            if (clientSelectionne != null)
            {
                clientSelectionne.RemoveDecoder(decoderId);
                clientDao.UpdateClientDecodeurs(clientId, clientSelectionne.Decoders);
            }

            return RedirectToAction("Details", "Client", new { id = clientId });
        }

        /// 
        /// Affiche la page détaillée d'un décodeur.
        /// L'utilisateur doit avoir accès au client auquel le décodeur appartient.
        /// 
        public IActionResult Details(int id)
        {
            int? idUtilisateur = HttpContext.Session.GetInt32("UserId");
            string? roleUtilisateur = HttpContext.Session.GetString("Role");

            if (idUtilisateur == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            UserDao userDao = new UserDao();
            DecodeurDao decodeurDao = new DecodeurDao();

            User? utilisateurCourant = userDao.SelectUserById(idUtilisateur.Value);
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (utilisateurCourant == null || decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            bool accesAutorise = roleUtilisateur == "Admin" ||
                                 utilisateurCourant.AccessibleClientIds.Contains(decodeurSelectionne.ClientId);

            if (!accesAutorise)
            {
                return RedirectToAction("Index", "Client");
            }

            ViewBag.IsAdmin = roleUtilisateur == "Admin";
            return View(decodeurSelectionne);
        }

        /// 
        /// Interroge l'API pour obtenir l'état actuel du décodeur.
        /// 
        public async Task<IActionResult> Info(int id)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            string resultatApi = await _serviceApiDecodeur.SendActionAsync(decodeurSelectionne.Address, "info");
            TempData["Message"] = resultatApi;

            return RedirectToAction("Details", new { id });
        }

        /// 
        /// Envoie une commande de redémarrage au décodeur via l'API.
        /// Correspond à l'action "reset" du simulateur.
        /// 
        public async Task<IActionResult> Reboot(int id)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            string resultatApi = await _serviceApiDecodeur.SendActionAsync(decodeurSelectionne.Address, "reset");
            TempData["Message"] = resultatApi;

            return RedirectToAction("Details", new { id });
        }

        /// 
        /// Envoie une commande de réinitialisation au décodeur via l'API.
        /// Correspond à l'action "reinit" du simulateur.
        /// 
        public async Task<IActionResult> Reset(int id)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            string resultatApi = await _serviceApiDecodeur.SendActionAsync(decodeurSelectionne.Address, "reinit");
            TempData["Message"] = resultatApi;

            return RedirectToAction("Details", new { id });
        }

        /// 
        /// Envoie une commande d'arrêt au décodeur via l'API.
        /// Correspond à l'action "shutdown" du simulateur.
        /// 
        public async Task<IActionResult> Shutdown(int id)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            string resultatApi = await _serviceApiDecodeur.SendActionAsync(decodeurSelectionne.Address, "shutdown");
            TempData["Message"] = resultatApi;

            return RedirectToAction("Details", new { id });
        }

        /// 
        /// Ajoute une chaîne à la liste locale du décodeur.
        /// 
        [HttpPost]
        public IActionResult AddChannel(int id, string channel)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            bool operationReussie = decodeurSelectionne.AddChannel(channel);
            
            if (operationReussie)
            {
                decodeurDao.UpdateChannels(id, decodeurSelectionne.Channels);
            }

            TempData["Message"] = operationReussie
                ? "La chaîne a été ajoutée avec succès."
                : "Impossible d'ajouter cette chaîne.";

            return RedirectToAction("Details", new { id });
        }

        /// 
        /// Retire une chaîne de la liste locale du décodeur.
        /// 
        [HttpPost]
        public IActionResult RemoveChannel(int id, string channel)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeurSelectionne = decodeurDao.SelectDecodeurById(id);

            if (decodeurSelectionne == null)
            {
                return RedirectToAction("Index", "Client");
            }

            bool operationReussie = decodeurSelectionne.RemoveChannel(channel);
            
            if (operationReussie)
            {
                decodeurDao.UpdateChannels(id, decodeurSelectionne.Channels);
            }

            TempData["Message"] = operationReussie
                ? "La chaîne a été retirée avec succès."
                : "La chaîne demandée est introuvable.";

            return RedirectToAction("Details", new { id });
        }
        
        
        ///
        /// Récupère le status
        ///
        [HttpGet]
        public async Task<IActionResult> GetStatus(int id)
        {
            DecodeurDao decodeurDao = new DecodeurDao();
            Decodeur? decodeur = decodeurDao.SelectDecodeurById(id);

            if (decodeur == null)
                return NotFound();

            string json = await _serviceApiDecodeur.SendActionAsync(decodeur.Address, "info");

            var parsed = JsonSerializer.Deserialize<JsonElement>(json);

            return Json(parsed);
        }
    }
}
