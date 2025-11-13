
using Veiling.Server;
using Veiling.Server.Models;

public static class AppDbSeeder {
    public static void Seed(AppDbContext context) {
        if (context.Kavels.Any()) return;

        var royalFlora = new Bedrijf {
            Bedrijfscode = 0,
            Bedrijfsnaam = "RoyalFlora BV",
            KVKnummer = 13414514,
        };
        
        context.Bedrijven.Add(
            royalFlora
        );
        
        var leverancier = new Leverancier {
            BedrijfId = 0,
            Bedrijf = royalFlora,
            IndexOfReliabilityOfInformation = "A",
        };

        context.Leveranciers.Add(
            leverancier
        );

        context.Kavels.Add(
            new Kavel {
                Id = 0,
                Naam = "Tulpen",
                Beschrijving = "In 1962 werd ik aangevallen door beren.<br/>Nu vertrouw ik dus geen beren meer.",
                MinimumPrijs = 0.22F,
                MaximumPrijs = 0.97F,
                Kavelkleur = "87335F",
                Leverancier = leverancier,
                LeverancierId = leverancier.Id,
                StageOfMaturity = "24",
                Keurcode = "A2",
                Fustcode = 13914,
            }
        );

        context.Kavels.Add(
            new Kavel {
                Id = 0,
                Naam = "Rozen",
                Beschrijving = "Hey Vsauce, Michael here.",
                MinimumPrijs = 0.12F,
                MaximumPrijs = 0.40F,
                Kavelkleur = "AAFF33",
                Leverancier = leverancier,
                LeverancierId = leverancier.Id,
                StageOfMaturity = "20",
                Keurcode = "A1",
                Fustcode = 12354,
            }
        );

        context.SaveChanges();
    }
}
