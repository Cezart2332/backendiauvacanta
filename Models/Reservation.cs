namespace IauVacanta.Backend.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int PlaceId { get; set; }
        public Place? Place { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}