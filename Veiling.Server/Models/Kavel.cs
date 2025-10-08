﻿namespace Veiling.Server.Models
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
        public float GekochPrijs { get; set; }
        public int Minimumhoeveelheid { get; set; }
        public string Foto { get; set; } = string.Empty;
        public string Kavelkleur { get; set; } = string.Empty; // hex code
        public int Karnummer { get; set; }
        public int Rijnummer { get; set; }
        public int HoeveelheidContainers { get; set; }
        public int AantalProductenPerContainer { get; set; }
        public float LengteVanBloemen { get; set; } // in centimeters
        public float GewichtVanBloemen { get; set; } // in grams
        public string StageOfMaturity { get; set; } = string.Empty;
        public char NgsCode { get; set; }
        public string Keurcode { get; set; } = string.Empty;
        public int Fustcode { get; set; }
        public string GeldPerTickCode { get; set; } = string.Empty;
        
        // Foreign Keys
        public int? GebruikerId { get; set; }
        public int? VeilingId { get; set; }
        public int? Bedrijfscode { get; set; }
        public int? LeverancierId { get; set; }
        
        // Relaties
        public Gebruiker? Gebruiker { get; set; }
        public VeilingItem? Veiling { get; set; }  
        public Bedrijf? Bedrijf { get; set; }
        public Leverancier? Leverancier { get; set; }
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
    }
}