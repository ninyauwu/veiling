namespace Veiling.Server.Models
{
    public class Kavel
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string Beschrijving { get; set; } = string.Empty;
        public string ArtikelKenmerken { get; set; } = string.Empty; 
        public int GekochteContainers { get; set; }
        public float MinimumPrijs { get; set; }
        public float MaximumPrijs { get; set; }
        public float GekochtPrijs { get; set; }
        public int Minimumhoeveelheid { get; set; }
        public string Foto { get; set; } = string.Empty;
        public string Kavelkleur { get; set; } = string.Empty; 
        public int Karnummer { get; set; }
        public int Rijnummer { get; set; }
        public int HoeveelheidContainers { get; set; } 
        public int AantalProductenPerContainer { get; set; }
        public float LengteVanBloemen { get; set; } 
        public float GewichtVanBloemen { get; set; } 
        public string StageOfMaturity { get; set; } = string.Empty; 
        public char NgsCode { get; set; }
        public string Keurcode { get; set; } = string.Empty;
        public int Fustcode { get; set; }
        public string GeldPerTickCode { get; set; } = string.Empty;
        public bool? Approved { get; set; } = null;
        public string? Reasoning { get; set; } = null;
        
        // Foreign Keys
        public int? VeilingId { get; set; }
        public int? LeverancierId { get; set; }
        
        // Relaties
        public Veiling? Veiling { get; set; }   
        public Leverancier? Leverancier { get; set; }
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
    }
}