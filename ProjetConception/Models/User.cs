using System.Collections.Generic;

namespace ProjetConception.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<int> AccessibleClientIds { get; set; }

        public User(int id, string name, string username, string password, Role role)
        {
            Id = id;
            Name = name;
            Username = username;
            Password = password;
            Role = role;
            AccessibleClientIds = new List<int>();
        }
    }
}