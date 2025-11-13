namespace Veiling.Server.Models
{
    public class Leverancier
    {
        public int Id { get; set; }
        public string IndexOfReliabilityOfInformation { get; set; } = string.Empty;
        
        public int? BedrijfId { get; set; }
        public Bedrijf? Bedrijf { get; set; }
        
        // Relaties
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}