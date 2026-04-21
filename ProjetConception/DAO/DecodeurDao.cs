using Microsoft.Data.SqlClient;
using ProjetConception.Models;

namespace ProjetConception.DAO
{
    public class DecodeurDao : BaseDao
    {
        public Decodeur? SelectDecodeurById(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT * FROM [Decodeur] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = command.ExecuteReader();
            
            Decodeur decodeur = new Decodeur(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3));

            if (!reader.IsDBNull(4) &&
                Enum.TryParse(reader.GetString(4), true, out DecoderStatus status))
            {
                decodeur.Status = status;
            }

            if (!reader.IsDBNull(5))
            {
                string[] channels = reader.GetString(5)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string channel in channels)
                {
                    decodeur.AddChannel(channel.Trim());
                }
            }
            return decodeur;
        }


        public int InsertDecodeur(Decodeur decodeur)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = @"
                INSERT INTO [Decodeur] (id, serialnumber, clientId, address, status, channels)
                VALUES (@id, @serialnumber, @clientId, @address, @status, @channels)";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", decodeur.Id);
            command.Parameters.AddWithValue("@serialnumber", decodeur.SerialNumber);
            command.Parameters.AddWithValue("@clientId", decodeur.ClientId);
            command.Parameters.AddWithValue("@address", decodeur.Address);
            command.Parameters.AddWithValue("@status", decodeur.Status.ToString());

            string channelsText = string.Join(";", decodeur.Channels);
            command.Parameters.AddWithValue("@channels", channelsText);

            return command.ExecuteNonQuery();
        }

        public int DeleteDecodeur(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "DELETE FROM [Decodeur] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery();
        }

        public int UpdateSerialNumber(int id, string serialNumber)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Decodeur] SET serialnumber = @serialnumber WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@serialnumber", serialNumber);

            return command.ExecuteNonQuery();
        }

        public int UpdateClientId(int id, int clientId)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Decodeur] SET clientId = @clientId WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@clientId", clientId);

            return command.ExecuteNonQuery();
        }

        public int UpdateAddress(int id, string address)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Decodeur] SET address = @address WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@address", address);

            return command.ExecuteNonQuery();
        }

        public int UpdateStatus(int id, DecoderStatus status)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Decodeur] SET status = @status WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@status", status.ToString());

            return command.ExecuteNonQuery();
        }

        public int UpdateChannels(int id, List<string> channels)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [Decodeur] SET channels = @channels WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            string channelsText = string.Join(";", channels);
            command.Parameters.AddWithValue("@channels", channelsText);

            return command.ExecuteNonQuery();
        }
        
        public int GetNextDecodeurId()
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT ISNULL(MAX(id), 0) + 1 FROM [Decodeur]";

            using SqlCommand command = new SqlCommand(sql, connection);

            return (int)command.ExecuteScalar();
        }
    }
}