using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class KavelsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public KavelsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<Leverancier> CreateTestLeverancier()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test Bloemen BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var leverancier = new Leverancier
            {
                BedrijfId = createdBedrijf!.Bedrijfscode,
                IndexOfReliabilityOfInformation = "A",
                Bedrijf = null
            };
            var response = await _client.PostAsJsonAsync("/api/leveranciers", leverancier);
            return (await response.Content.ReadFromJsonAsync<Leverancier>())!;
        }

        [Fact]
        public async Task CreateKavel_StoresAllFieldsCorrectly()
        {
            var leverancier = await CreateTestLeverancier();
            
            var kavel = new Kavel
            {
                Naam = "Rode Rozen Premium",
                Beschrijving = "Hoogwaardige rode rozen uit Nederland",
                ArtikelKenmerken = "Lang, sterk, geurig",
                MinimumPrijs = 15.50f,
                MaximumPrijs = 25.75f,
                Minimumhoeveelheid = 10,
                Foto = "/images/rozen.jpg",
                Kavelkleur = "FF0000",
                Karnummer = 5,
                Rijnummer = 3,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 20,
                LengteVanBloemen = 60.5f,
                GewichtVanBloemen = 500.25f,
                StageOfMaturity = "Bloeiend",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 123,
                GeldPerTickCode = "0.5",
                LeverancierId = leverancier.Id
            };

            var response = await _client.PostAsJsonAsync("/api/kavels", kavel);
            var created = await response.Content.ReadFromJsonAsync<Kavel>();

            // verify every field
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            
            // String fields
            Assert.Equal("Rode Rozen Premium", created.Naam);
            Assert.Equal("Hoogwaardige rode rozen uit Nederland", created.Beschrijving);
            Assert.Equal("Lang, sterk, geurig", created.ArtikelKenmerken);
            Assert.Equal("/images/rozen.jpg", created.Foto);
            Assert.Equal("FF0000", created.Kavelkleur);
            Assert.Equal("Bloeiend", created.StageOfMaturity);
            Assert.Equal("A1", created.Keurcode);
            Assert.Equal("0.5", created.GeldPerTickCode);
            
            // Numeric fields
            Assert.Equal(15.50f, created.MinimumPrijs);
            Assert.Equal(25.75f, created.MaximumPrijs);
            Assert.Equal(10, created.Minimumhoeveelheid);
            Assert.Equal(5, created.Karnummer);
            Assert.Equal(3, created.Rijnummer);
            Assert.Equal(50, created.HoeveelheidContainers);
            Assert.Equal(20, created.AantalProductenPerContainer);
            Assert.Equal(60.5f, created.LengteVanBloemen);
            Assert.Equal(500.25f, created.GewichtVanBloemen);
            Assert.Equal(123, created.Fustcode);
            
            // Char field
            Assert.Equal('A', created.NgsCode);
            
            // Relation
            Assert.Equal(leverancier.Id, created.LeverancierId);
        }

        [Fact]
        public async Task GetKavel_ReturnsExactSameData()
        {
            var leverancier = await CreateTestLeverancier();
            var originalKavel = new Kavel
            {
                Naam = "Witte Tulpen",
                Beschrijving = "Verse Nederlandse tulpen",
                ArtikelKenmerken = "Wit, groot",
                MinimumPrijs = 8.25f,
                MaximumPrijs = 12.50f,
                Minimumhoeveelheid = 25,
                Foto = "/images/tulpen.jpg",
                Kavelkleur = "FFFFFF",
                Karnummer = 2,
                Rijnummer = 7,
                HoeveelheidContainers = 100,
                AantalProductenPerContainer = 15,
                LengteVanBloemen = 45.0f,
                GewichtVanBloemen = 350.0f,
                StageOfMaturity = "Knop",
                NgsCode = 'B',
                Keurcode = "B2",
                Fustcode = 456,
                GeldPerTickCode = "1.0",
                LeverancierId = leverancier.Id
            };

            var createResponse = await _client.PostAsJsonAsync("/api/kavels", originalKavel);
            var created = await createResponse.Content.ReadFromJsonAsync<Kavel>();

            // retrieve it
            var getResponse = await _client.GetAsync($"/api/kavels/{created!.Id}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Kavel>();

            // verify data matches
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(retrieved);
            
            Assert.Equal("Witte Tulpen", retrieved.Naam);
            Assert.Equal("Verse Nederlandse tulpen", retrieved.Beschrijving);
            Assert.Equal(8.25f, retrieved.MinimumPrijs);
            Assert.Equal(12.50f, retrieved.MaximumPrijs);
            Assert.Equal(25, retrieved.Minimumhoeveelheid);
            Assert.Equal("FFFFFF", retrieved.Kavelkleur);
            Assert.Equal(2, retrieved.Karnummer);
            Assert.Equal(7, retrieved.Rijnummer);
            Assert.Equal(100, retrieved.HoeveelheidContainers);
            Assert.Equal(45.0f, retrieved.LengteVanBloemen);
            Assert.Equal('B', retrieved.NgsCode);
            Assert.Equal("B2", retrieved.Keurcode);
        }

        [Fact]
        public async Task UpdateKavel_ChangesMultipleFields()
        {
            var leverancier = await CreateTestLeverancier();
            var kavel = new Kavel
            {
                Naam = "Original Naam",
                Beschrijving = "Original beschrijving",
                ArtikelKenmerken = "Original kenmerken",
                MinimumPrijs = 10.0f,
                MaximumPrijs = 20.0f,
                Minimumhoeveelheid = 5,
                Foto = "/old.jpg",
                Kavelkleur = "000000",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 10,
                AantalProductenPerContainer = 10,
                LengteVanBloemen = 50.0f,
                GewichtVanBloemen = 400.0f,
                StageOfMaturity = "Oud",
                NgsCode = 'C',
                Keurcode = "C1",
                Fustcode = 100,
                GeldPerTickCode = "1.0",
                LeverancierId = leverancier.Id
            };

            var createResponse = await _client.PostAsJsonAsync("/api/kavels", kavel);
            var created = await createResponse.Content.ReadFromJsonAsync<Kavel>();

            // update multiple fields
            created!.Naam = "UPDATED NAAM";
            created.Beschrijving = "UPDATED BESCHRIJVING";
            created.MinimumPrijs = 99.99f;
            created.MaximumPrijs = 199.99f;
            created.HoeveelheidContainers = 999;
            created.Kavelkleur = "FF00FF";
            created.NgsCode = 'A';
            
            var updateResponse = await _client.PutAsJsonAsync($"/api/kavels/{created.Id}", created);

            // verify database changed
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/kavels/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Kavel>();
            
            Assert.NotNull(updated);
            Assert.Equal("UPDATED NAAM", updated.Naam);
            Assert.Equal("UPDATED BESCHRIJVING", updated.Beschrijving);
            Assert.Equal(99.99f, updated.MinimumPrijs);
            Assert.Equal(199.99f, updated.MaximumPrijs);
            Assert.Equal(999, updated.HoeveelheidContainers);
            Assert.Equal("FF00FF", updated.Kavelkleur);
            Assert.Equal('A', updated.NgsCode);
        }

        [Fact]
        public async Task GetKavelsByVeiling_FiltersCorrectly()
        {
            // create veiling first
            var veiling = new Models.Veiling
            {
                Naam = "Test Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };
            var veilingResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var createdVeiling = await veilingResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var leverancier = await CreateTestLeverancier();

            // Create kavels some with veiling, some without
            var kavelMetVeiling = new Kavel
            {
                Naam = "Kavel MET Veiling",
                Beschrijving = "Test",
                ArtikelKenmerken = "Test",
                MinimumPrijs = 10.0f,
                MaximumPrijs = 20.0f,
                Minimumhoeveelheid = 5,
                Foto = "/test.jpg",
                Kavelkleur = "FFFFFF",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 10,
                AantalProductenPerContainer = 10,
                LengteVanBloemen = 50.0f,
                GewichtVanBloemen = 400.0f,
                StageOfMaturity = "Test",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 100,
                GeldPerTickCode = "1.0",
                LeverancierId = leverancier.Id,
                VeilingId = createdVeiling!.Id
            };
            await _client.PostAsJsonAsync("/api/kavels", kavelMetVeiling);

            var response = await _client.GetAsync($"/api/kavels/veiling/{createdVeiling.Id}");
            var kavels = await response.Content.ReadFromJsonAsync<List<Kavel>>();

            // check it returns the right kavel
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(kavels);
            Assert.Contains(kavels, k => k.Naam == "Kavel MET Veiling");
            
            // Verify the kavel has correct veiling ID
            var foundKavel = kavels.First(k => k.Naam == "Kavel MET Veiling");
            Assert.Equal(createdVeiling.Id, foundKavel.VeilingId);
        }
    }
}

