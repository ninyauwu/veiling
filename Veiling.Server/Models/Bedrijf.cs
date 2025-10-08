﻿namespace Veiling.Server.Models
{
    public class Bedrijf
    {
        public int Bedrijfscode { get; set; }
        public string Bedrijfsnaam { get; set; } = string.Empty;
        public int KVKnummer { get; set; }
        
        // Relaties
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}