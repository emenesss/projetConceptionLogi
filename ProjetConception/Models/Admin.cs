namespace ProjetConception.Models
{
    public class Admin : User
    {
        public Admin(int id, string name, string username, string password)
            : base(id, name, username, password, Role.Admin)
        {
        }
    }
}