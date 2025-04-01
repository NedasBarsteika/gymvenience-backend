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
        public bool IsTrainer { get; set; } = false; 
        public string Bio { get; set; } = string.Empty; 
        public List<Product> PurchasedProducts { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            Name = "";
            Surname = "";
            Email = "";
            HashedPassword = "";
            Salt = "";
            IsAdmin = false;
            IsTrainer = false;
            Bio = string.Empty;
            PurchasedProducts = new List<Product>();
        }

        // Optional: Extend your parameterized constructor if needed
        public User(string id, string name, string surname, string email, string password, string salt)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            HashedPassword = password;
            Salt = salt;
            IsAdmin = false;
            IsTrainer = false;
            Bio = "";
            PurchasedProducts = new List<Product>();
        }
    }
}
