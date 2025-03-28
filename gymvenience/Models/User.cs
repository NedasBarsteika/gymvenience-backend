namespace gymvenience_backend.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; }
        public string HashedPassword { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public List<Product> PurchasedProducts { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            Name = "";
            Surname = "";
            Email = "";
            HashedPassword = "";
            Salt = "";
            PurchasedProducts = new List<Product>();
        }

        public User(string id, string name, string surname, string email, string password, string salt)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            HashedPassword = password;
            Salt = salt;
            PurchasedProducts = new List<Product>();
        }
    }
}
