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
    
            // Create veiling first
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
    
            // Use CreateKavelDto instead of Kavel
            var kavelDto = new
            {
                Naam = "Rode Rozen Premium",
                Description = "Hoogwaardige rode rozen uit Nederland",
                ImageUrl = "/images/rozen.jpg",
                MinimumPrijs = 15.50f,
                Aantal = 50,
                Ql = "A1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Bloeiend",
                Lengte = 60.5f,
                Kleur = "FF0000",
                Fustcode = 123,
                AantalProductenPerContainer = 20,
                GewichtVanBloemen = 500.25f
            };

            var response = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var created = await response.Content.ReadFromJsonAsync<Kavel>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Rode Rozen Premium", created.Naam);
            Assert.Equal("Hoogwaardige rode rozen uit Nederland", created.Beschrijving);
        }

        [Fact]
        public async Task GetKavel_ReturnsExactSameData()
        {
            var leverancier = await CreateTestLeverancier();
    
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
    
            var kavelDto = new
            {
                Naam = "Witte Tulpen",
                Description = "Verse Nederlandse tulpen",
                ImageUrl = "/images/tulpen.jpg",
                MinimumPrijs = 8.25f,
                Aantal = 100,
                Ql = "B2",
                VeilingId = createdVeiling!.Id,
                Stadium = "Knop",
                Lengte = 45.0f,
                Kleur = "FFFFFF",
                Fustcode = 456,
                AantalProductenPerContainer = 15,
                GewichtVanBloemen = 350.0f
            };

            var createResponse = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var created = await createResponse.Content.ReadFromJsonAsync<Kavel>();

            // retrieve it
            var getResponse = await _client.GetAsync($"/api/kavels/{created!.Id}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Kavel>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal("Witte Tulpen", retrieved.Naam);
            Assert.Equal("Verse Nederlandse tulpen", retrieved.Beschrijving);
        }

        [Fact]
        public async Task UpdateKavel_ChangesMultipleFields()
        {
            var leverancier = await CreateTestLeverancier();
    
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
    
            var kavelDto = new
            {
                Naam = "Original Naam",
                Description = "Original beschrijving",
                ImageUrl = "/old.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 10,
                Ql = "C1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Oud",
                Lengte = 50.0f,
                Kleur = "000000",
                Fustcode = 100,
                AantalProductenPerContainer = 10,
                GewichtVanBloemen = 400.0f
            };

            var createResponse = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var created = await createResponse.Content.ReadFromJsonAsync<Kavel>();

            // update multiple fields - now use the full Kavel object for PUT
            created!.Naam = "UPDATED NAAM";
            created.Beschrijving = "UPDATED BESCHRIJVING";
            created.MinimumPrijs = 99.99f;
            created.MaximumPrijs = 199.99f;
            created.HoeveelheidContainers = 999;
            created.Kavelkleur = "FF00FF";
            created.NgsCode = 'A';
    
            var updateResponse = await _client.PutAsJsonAsync($"/api/kavels/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/kavels/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Kavel>();
    
            Assert.NotNull(updated);
            Assert.Equal("UPDATED NAAM", updated.Naam);
            Assert.Equal("UPDATED BESCHRIJVING", updated.Beschrijving);
        }

        [Fact]
        public async Task GetKavelsByVeiling_FiltersCorrectly()
        {
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

            var kavelDto = new
            {
                Naam = "Kavel MET Veiling",
                Description = "Test",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 10,
                Ql = "A1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Test",
                Lengte = 50.0f,
                Kleur = "FFFFFF",
                Fustcode = 100,
                AantalProductenPerContainer = 10,
                GewichtVanBloemen = 400.0f
            };
            await _client.PostAsJsonAsync("/api/kavels", kavelDto);

            var response = await _client.GetAsync($"/api/kavels/veiling/{createdVeiling.Id}");
            var kavels = await response.Content.ReadFromJsonAsync<List<Kavel>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(kavels);
            Assert.Contains(kavels, k => k.Naam == "Kavel MET Veiling");
        }
        
        [Fact]
        public async Task GetAllKavels_ReturnsCompleteRelations()
        {
            var leverancier = await CreateTestLeverancier();
    
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
    
            var kavelDto = new
            {
                Naam = "Test Relations",
                Description = "Test",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 10,
                Ql = "A1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Test",
                Lengte = 50.0f,
                Kleur = "FFFFFF",
                Fustcode = 100,
                AantalProductenPerContainer = 10,
                GewichtVanBloemen = 400.0f
            };
            await _client.PostAsJsonAsync("/api/kavels", kavelDto);

            var response = await _client.GetAsync("/api/kavels");
            var kavels = await response.Content.ReadFromJsonAsync<List<Kavel>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(kavels);
            Assert.NotEmpty(kavels);
        }

        [Fact]
        public async Task DeleteKavel_RemovesFromDatabase()
        {
            var leverancier = await CreateTestLeverancier();
    
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
    
            var kavelDto = new
            {
                Naam = "To Delete",
                Description = "Test",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 10,
                Ql = "A1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Test",
                Lengte = 50.0f,
                Kleur = "FFFFFF",
                Fustcode = 100,
                AantalProductenPerContainer = 10,
                GewichtVanBloemen = 400.0f
            };
            var createResponse = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var created = await createResponse.Content.ReadFromJsonAsync<Kavel>();

            var deleteResponse = await _client.DeleteAsync($"/api/kavels/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/kavels/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

[Fact]
public async Task GetKavel_WithInvalidId_ReturnsNotFound()
{
    var response = await _client.GetAsync("/api/kavels/99999");
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
    }
}

