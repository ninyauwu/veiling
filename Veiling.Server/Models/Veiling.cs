using System.Text.Json.Serialization;

namespace Veiling.Server.Models
{
    public class Veiling 
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public float Klokduur { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EndTijd { get; set; }
        public float GeldPerTickCode { get; set; }
        
        public int? VeilingmeesterId { get; set; }
        public int? LocatieId { get; set; }
        
        [JsonIgnore] 
        public Veilingmeester? Veilingmeester { get; set; }
        
        [JsonIgnore] 
        public Locatie? Locatie { get; set; }
        
        [JsonIgnore]
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}