namespace IauVacanta.Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public Profile? Profile { get; set; }
        public ICollection<Place> Places { get; set; } = new List<Place>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}