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
                    new Models.Locatie { Naam = "Rotterdam", KlokId = 2, Actief = true },
                    new Models.Locatie { Naam = "Delft", KlokId = 3, Actief = true }
                );
                context.SaveChanges();
            }
        }
    }
}