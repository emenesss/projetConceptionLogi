namespace ProjetConception.Models
{
    
    /// Source temporaire de données utilisée pour le prototype.
    /// Les données sont conservées en mémoire pendant l'exécution. (base de données)
    
    public static class DataStore
    {
        public static ISP ISP = InitialiserDonnees();

        private static ISP InitialiserDonnees()
        {
            ISP fournisseur = new ISP();

            // Utilisateurs de départ
            Admin administrateur = new Admin(1, "Administrateur principal", "admin", "1234");
            EndUser utilisateurClient = new EndUser(2, "Utilisateur client", "user", "1234");

            fournisseur.Users.Add(administrateur);
            fournisseur.Users.Add(utilisateurClient);

            // Clients de départ
            Client clientA = new Client(1, "Client A");
            Client clientB = new Client(2, "Client B");

            fournisseur.Clients.Add(clientA);
            fournisseur.Clients.Add(clientB);

            // Décodeurs liés aux adresses IP du simulateur
            Decodeur decodeurA1 = new Decodeur(1, "DEC-A01", 1, "127.0.10.1");
            Decodeur decodeurA2 = new Decodeur(2, "DEC-A02", 1, "127.0.10.2");
            Decodeur decodeurB1 = new Decodeur(3, "DEC-B01", 2, "127.0.10.3");

            // Chaînes de départ
            decodeurA1.AddChannel("RDS");
            decodeurA1.AddChannel("TVA");
            decodeurA2.AddChannel("Noovo");
            decodeurB1.AddChannel("Canal D");

            clientA.AddDecoder(decodeurA1);
            clientA.AddDecoder(decodeurA2);
            clientB.AddDecoder(decodeurB1);

            // L'utilisateur final a accès seulement au premier client au départ
            utilisateurClient.AccessibleClientIds.Add(clientA.Id);

            return fournisseur;
        }
    }
}