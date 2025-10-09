namespace Veiling.Server.Models
{
    public class VeilingItem  // ✅ Geen conflict!
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public int KlokId { get; set; }
        public float Klokduur { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EndTijd { get; set; }
        public float GeldPerTickCode { get; set; }
        
        public int? VeilingmeesterId { get; set; }

        public Veilingmeester? Veilingmeester { get; set; }
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}