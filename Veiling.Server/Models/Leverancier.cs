namespace Veiling.Server.Models
{
    public class Leverancier
    {
        public int Id { get; set; }
        public string IndexOfReliabilityOfInformation { get; set; } = string.Empty;
        
        // Relaties
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}