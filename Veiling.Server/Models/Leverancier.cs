using System.Text.Json.Serialization;

namespace Veiling.Server.Models
{
    public class Leverancier
    {
        public int Id { get; set; }
        public string IndexOfReliabilityOfInformation { get; set; } = string.Empty;
        
        public int BedrijfId { get; set; }
        
        // Relaties
        public required Bedrijf? Bedrijf { get; set; }
        [JsonIgnore]
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}
