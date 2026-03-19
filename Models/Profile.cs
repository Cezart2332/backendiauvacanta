namespace IauVacanta.Backend.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}