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
        public int? GebruikerId { get; set; }
        public int? KavelId { get; set; }
        
        // Relaties
        public Gebruiker? Gebruiker { get; set; }
        public Kavel? Kavel { get; set; }
    }
}