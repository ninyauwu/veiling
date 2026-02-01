namespace Veiling.Server.Models
{
    public class Bod
    {
        public int Id { get; set; }
        public DateTime Datumtijd { get; set; }
        public int HoeveelheidContainers { get; set; }
        public float Koopprijs { get; set; }
        public bool Betaald { get; set; }
        
        // Foreign Keys
        public string? GebruikerId { get; set; }
        public int? KavelVeilingId { get; set; }
        public int? AankoopId { get; set; }
        
        // Relaties
        public Gebruiker? Gebruiker { get; set; }
        public KavelVeiling? KavelVeiling { get; set; }
        public Aankoop? Aankoop { get; set; }
    }
}
