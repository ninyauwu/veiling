namespace Veiling.Server
{
    public static class AppDbSeeder 
    {
        public static void Seed(AppDbContext context) 
        {
            if (!context.Locaties.Any())
            {
                context.Locaties.AddRange(
                    new Models.Locatie { Naam = "Amsterdam", KlokId = 1, Actief = true },
                    new Models.Locatie { Naam = "Rotterdam", KlokId = 2, Actief = false },
                    new Models.Locatie { Naam = "Delft", KlokId = 3, Actief = true }
                );
            }
            else
            {
                // Update bestaande data
                var amsterdam = context.Locaties.FirstOrDefault(l => l.Naam == "Amsterdam");
                if (amsterdam != null) amsterdam.Actief = true;
        
                var rotterdam = context.Locaties.FirstOrDefault(l => l.Naam == "Rotterdam");
                if (rotterdam != null) rotterdam.Actief = false;
        
                var delft = context.Locaties.FirstOrDefault(l => l.Naam == "Delft");
                if (delft != null) delft.Actief = true;
            }
    
            context.SaveChanges();
        }
    }
}