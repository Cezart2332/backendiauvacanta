namespace IauVacanta.Backend.Models
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}