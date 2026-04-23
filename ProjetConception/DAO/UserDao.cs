using Microsoft.Data.SqlClient;
using ProjetConception.Models;

namespace ProjetConception.DAO
{
    public class UserDao : BaseDao
    {
        public User? SelectUserByUsernameAndPassword(string username, string password)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT id, name, username, password, role, accessibleClientIds FROM [User] WHERE username = @username AND password = @password";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            Role role = Role.EndUser;

            if (!reader.IsDBNull(4) &&
                Enum.TryParse(reader.GetString(4), true, out Role parsedRole))
            {
                role = parsedRole;
            }

            User user = new User(
                reader.GetInt32(0),
                reader.GetString(1).Trim(),
                reader.GetString(2).Trim(),
                reader.GetString(3),
                role
            );

            if (!reader.IsDBNull(5))
            {
                string accessibleClientIdsText = reader.GetString(5);
                string[] clientIds = accessibleClientIdsText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string clientIdText in clientIds)
                {
                    if (int.TryParse(clientIdText.Trim(), out int clientId))
                    {
                        user.AccessibleClientIds.Add(clientId);
                    }
                }
            }
            return user;
        }
        public User? SelectUserById(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT * FROM [User] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            Role role = Role.EndUser;

            if (!reader.IsDBNull(4) &&
                Enum.TryParse(reader.GetString(4), true, out Role parsedRole))
            {
                role = parsedRole;
            }

            User user = new User(reader.GetInt32(0), reader.GetString(1).Trim(), reader.GetString(2).Trim(), reader.GetString(3), role);
            
            if (!reader.IsDBNull(5))
            {
                string accessibleClientIdsText = reader.GetString(5);
                string[] clientIds = accessibleClientIdsText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string clientIdText in clientIds)
                {
                    if (int.TryParse(clientIdText.Trim(), out int clientId))
                    {
                        user.AccessibleClientIds.Add(clientId);
                    }
                }
            }
            return user;
        }

        public int InsertUser(User user)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = @"
                INSERT INTO [User] (id, name, username, password, role, accessibleClientIds)
                VALUES (@id, @name, @username, @password, @role, @accessibleClientIds)";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@role", user.Role.ToString());

            string accessibleClientIdsText = string.Join(";", user.AccessibleClientIds);
            command.Parameters.AddWithValue(
                "@accessibleClientIds",
                string.IsNullOrWhiteSpace(accessibleClientIdsText)
                    ? DBNull.Value
                    : accessibleClientIdsText
            );
            return command.ExecuteNonQuery();
        }
        
        public int DeleteUser(int id)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "DELETE FROM [User] WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery();
        }
        
        public int UpdateUserName(int id, string name)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [User] SET name = @name WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);

            return command.ExecuteNonQuery();
        }

        public int UpdateUsername(int id, string username)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [User] SET username = @username WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery();
        }

        public int UpdatePassword(int id, string password)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [User] SET password = @password WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@password", password);

            return command.ExecuteNonQuery();
        }

        public int UpdateRole(int id, Role role)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [User] SET role = @role WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@role", role.ToString());

            return command.ExecuteNonQuery();
        }

        public int UpdateAccessibleClientIds(int id, List<int> accessibleClientIds)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "UPDATE [User] SET accessibleClientIds = @accessibleClientIds WHERE id = @id";

            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            string accessibleClientIdsText = string.Join(";", accessibleClientIds);
            command.Parameters.AddWithValue(
                "@accessibleClientIds",
                string.IsNullOrWhiteSpace(accessibleClientIdsText)
                    ? DBNull.Value
                    : accessibleClientIdsText
            );

            return command.ExecuteNonQuery();
        }
        
        public List<User> SelectAllUsers()
        {
            List<User> users = new List<User>();

            using SqlConnection connection = GetConnection();
            connection.Open();

            string sql = "SELECT id, name, username, password, role, accessibleClientIds FROM [User]";

            using SqlCommand command = new SqlCommand(sql, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Role role = Role.EndUser;

                if (!reader.IsDBNull(4) &&
                    Enum.TryParse(reader.GetString(4), true, out Role parsedRole))
                {
                    role = parsedRole;
                }

                User user = new User(reader.GetInt32(0), reader.GetString(1).Trim(), reader.GetString(2).Trim(), reader.GetString(3), role);

                if (!reader.IsDBNull(5))
                {
                    string accessibleClientIdsText = reader.GetString(5);
                    string[] clientIds = accessibleClientIdsText.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string clientIdText in clientIds)
                    {
                        if (int.TryParse(clientIdText.Trim(), out int clientId))
                        {
                            user.AccessibleClientIds.Add(clientId);
                        }
                    }
                }
                users.Add(user);
            }
            return users;
        }
    }
}