using Microsoft.Data.SqlClient;
using ProjetConception.Models;

namespace ProjetConception.DAO
{
    public class ClientDao : BaseDao
    {
        public Client? SelectClientById(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT * FROM [Client] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = command.ExecuteReader();
            
            if (!reader.Read())
                return null;
            
            Client client = new Client(reader.GetInt32(0), reader.GetString(1).Trim());

            string? decodeursText = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? authorizedUsersText = reader.IsDBNull(3) ? null : reader.GetString(3);

            reader.Close();

            DecodeurDao decodeurDao = new DecodeurDao();

            if (!string.IsNullOrWhiteSpace(decodeursText))
            {
                string[] decodeurIds = decodeursText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string decodeurIdText in decodeurIds)
                {
                    if (int.TryParse(decodeurIdText.Trim(), out int decodeurId))
                    {
                        Decodeur? decodeur = decodeurDao.SelectDecodeurById(decodeurId);

                        if (decodeur != null)
                        {
                            client.AddDecoder(decodeur);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(authorizedUsersText))
            {
                string[] userIds = authorizedUsersText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string userIdText in userIds)
                {
                    if (int.TryParse(userIdText.Trim(), out int userId))
                    {
                        client.AuthorizedUserIds.Add(userId);
                    }
                }
            }
            return client;
        }

        public List<Client> SelectAllClients()
        {
            List<Client> clients = new List<Client>();
            
            using SqlConnection connection = GetConnection();
            connection.Open();
            
            String sql = "SELECT * FROM [Client]";
            
            using SqlCommand command = new SqlCommand(sql, connection);
            using SqlDataReader reader = command.ExecuteReader();
            
            DecodeurDao decodeurDao = new DecodeurDao();
            
            while (reader.Read())
            {
                Client client = new Client(
                    reader.GetInt32(0),
                    reader.GetString(1).Trim()
                );

                if (!reader.IsDBNull(2))
                {
                    string decodeursText = reader.GetString(2);
                    string[] decodeurIds = decodeursText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string decodeurIdText in decodeurIds)
                    {
                        if (int.TryParse(decodeurIdText.Trim(), out int decodeurId))
                        {
                            Decodeur? decodeur = decodeurDao.SelectDecodeurById(decodeurId);
                            
                            if (decodeur != null)
                            {
                                client.AddDecoder(decodeur);
                            }
                        }
                    }
                }

                if (!reader.IsDBNull(3))
                {
                    string authorizedUserIdsText = reader.GetString(3);
                    string[] userIds = authorizedUserIdsText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string userIdText in userIds)
                    {
                        if (int.TryParse(userIdText.Trim(), out int userId))
                        {
                            client.AuthorizedUserIds.Add(userId);
                        }
                    }
                }
                clients.Add(client);
            }
            return clients;
        }

        public int InsertClient(Client client)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = @"
                INSERT INTO [Client] (id, name, decodeurs, authorizedUserids)
                VALUES (@id, @name, @decodeurs, @authorizedUserids)";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", client.Id);
            command.Parameters.AddWithValue("@name", client.Name);

            string decodeursText = string.Join(";", client.Decoders.Select(d => d.Id));
            string authorizedUsersText = string.Join(";", client.AuthorizedUserIds);

            command.Parameters.AddWithValue("@decodeurs", string.IsNullOrWhiteSpace(decodeursText) ? DBNull.Value : decodeursText);
            command.Parameters.AddWithValue("@authorizedUserids", string.IsNullOrWhiteSpace(authorizedUsersText) ? DBNull.Value : authorizedUsersText);

            return command.ExecuteNonQuery();
        }
        
        public int UpdateClientName(int id, string name)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Client] SET name = @name WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);

            return command.ExecuteNonQuery();
        }

        public int UpdateClientDecodeurs(int id, List<Decodeur> decodeurs)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Client] SET decodeurs = @decodeurs WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            string decodeursText = string.Join(";", decodeurs.Select(d => d.Id));
            command.Parameters.AddWithValue("@decodeurs", string.IsNullOrWhiteSpace(decodeursText) ? DBNull.Value : decodeursText);

            return command.ExecuteNonQuery();
        }

        public int UpdateAuthorizedUserIds(int id, List<int> authorizedUserIds)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Client] SET authorizedUserids = @authorizedUserids WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            string authorizedUsersText = string.Join(";", authorizedUserIds);
            command.Parameters.AddWithValue("@authorizedUserids", string.IsNullOrWhiteSpace(authorizedUsersText) ? DBNull.Value : authorizedUsersText);

            return command.ExecuteNonQuery();
        }
        
        public int DeleteClient(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "DELETE FROM [Client] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery();
        }
        
        public bool ClientIdExists(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT IIF(EXISTS (SELECT 1 FROM [Client] WHERE id = @id), 1, 0)";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            return (int)command.ExecuteScalar() == 1;
        }
        
    }
}