namespace ProjetConception.Models
{
    public class EndUser : User
    {
        public EndUser(int id, string name, string username, string password)
            : base(id, name, username, password, Role.EndUser)
        {
        }
    }
}