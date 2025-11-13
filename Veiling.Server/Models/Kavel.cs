namespace Veiling.Server.Models
{
    public class Kavel
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string? Beschrijving { get; set; }  
        public decimal StartPrijs { get; set; }     
        public int Aantal { get; set; }             
        public string? Kwaliteit { get; set; }      
        public string? PlaatsVanVerkoop { get; set; }
        public string? Stadium { get; set; }        
        public string? Lengte { get; set; }  
        public string? Kleur { get; set; }   
        public string? Fustcode { get; set; }

        public string? AfbeeldingUrl { get; set; }
        
        // Foreign Keys
        public int? VeilingId { get; set; }
        public int? LeverancierId { get; set; }
        
        // Relaties
        public Veiling? Veiling { get; set; }   
        public Leverancier? Leverancier { get; set; }
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
    }
}