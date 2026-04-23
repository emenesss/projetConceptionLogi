using System.Collections.Generic;
using System.Linq;

namespace ProjetConception.Models
{
    public class ISP
    {
        public List<Client> Clients { get; set; }
        public List<User> Users { get; set; }

        public ISP()
        {
            Clients = new List<Client>();
            Users = new List<User>();
        }

        public User? Authenticate(string username, string password)
        {
            return Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public Client? FindClientById(int clientId)
        {
            return Clients.FirstOrDefault(c => c.Id == clientId);
        }

        public User? FindUserById(int userId)
        {
            return Users.FirstOrDefault(u => u.Id == userId);
        }

        public bool AddClient(Client client)
        {
            if (Clients.Any(c => c.Id == client.Id))
                return false;

            Clients.Add(client);
            return true;
        }

        public bool RemoveClient(int clientId)
        {
            var client = FindClientById(clientId);
            if (client == null)
                return false;

            return Clients.Remove(client);
        }

        public bool AssignDecoderToClient(int clientId, Decodeur decoder)
        {
            var client = FindClientById(clientId);
            if (client == null)
                return false;

            client.AddDecoder(decoder);
            return true;
        }

        public bool GrantUserAccessToClient(int userId, int clientId)
        {
            var user = FindUserById(userId);
            var client = FindClientById(clientId);

            if (user == null || client == null)
                return false;

            if (!user.AccessibleClientIds.Contains(clientId))
                user.AccessibleClientIds.Add(clientId);

            if (!client.AuthorizedUserIds.Contains(userId))
                client.AuthorizedUserIds.Add(userId);

            return true;
        }

        public Decodeur? FindDecoderById(int decoderId)
        {
            foreach (var client in Clients)
            {
                var decoder = client.Decoders.FirstOrDefault(d => d.Id == decoderId);
                if (decoder != null)
                    return decoder;
            }

            return null;
        }
    }
}