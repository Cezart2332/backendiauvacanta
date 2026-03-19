namespace IauVacanta.Backend.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string City { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public int OwnerId { get; set; }
        public User? Owner { get; set; }

        public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}