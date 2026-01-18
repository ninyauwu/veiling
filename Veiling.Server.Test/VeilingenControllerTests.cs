using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class VeilingenControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public VeilingenControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateVeiling_WithValidData_ReturnsCreated()
        {
            var veiling = new Models.Veiling
            {
                Naam = "Amsterdam Ochtend Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow.AddHours(1),
                EndTijd = DateTime.UtcNow.AddHours(3),
                GeldPerTickCode = 0.5f,
                LocatieId = null
            };

            var response = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await response.Content.ReadFromJsonAsync<Models.Veiling>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(veiling.Naam, created.Naam);
        }

        [Fact]
        public async Task GetActieveVeilingen_ReturnsOnlyActiveOnes()
        {
            var veiling = new Models.Veiling
            {
                Naam = "Test Actieve Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow.AddHours(-1),
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };

            await _client.PostAsJsonAsync("/api/veilingen", veiling);

            var response = await _client.GetAsync("/api/veilingen/actief");
            var actieveVeilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveVeilingen);
            Assert.Contains(actieveVeilingen, v => v.Naam == "Test Actieve Veiling");
        }

        [Fact]
        public async Task UpdateVeiling_ChangesEndTime_Successfully()
        {
            var veiling = new Models.Veiling
            {
                Naam = "Update Test Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };

            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var newEndTime = DateTime.UtcNow.AddHours(4);
            created!.EndTijd = newEndTime;
            var updateResponse = await _client.PutAsJsonAsync($"/api/veilingen/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/veilingen/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            Assert.True(Math.Abs((updated!.EndTijd - newEndTime).TotalMinutes) < 1);
        }

        [Fact]
        public async Task GetActieveVeilingen_OnlyReturnsCurrentlyActiveOnes()
        {
            // Use DateTime.Now instead of DateTime.UtcNow to match controller
            var now = DateTime.Now;
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            // Create actieve veiling - give it a wider time window to avoid edge cases
            var actiefDto = new
            {
                Naam = $"Actief Nu {uniqueId}",
                StartTijd = now.AddHours(-2), // Started 2 hours ago
                EndTijd = now.AddHours(2), // Ends in 2 hours
                KavelIds = new List<int>()
            };
            var actiefResponse = await _client.PostAsJsonAsync("/api/veilingen", actiefDto);
            var actiefCreated = await actiefResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            Assert.NotNull(actiefCreated);
            Assert.True(actiefCreated.Id > 0);

            // Create toekomstige veiling
            var toekomstDto = new
            {
                Naam = $"Toekomst {uniqueId}",
                StartTijd = now.AddHours(3),
                EndTijd = now.AddHours(5),
                KavelIds = new List<int>()
            };
            await _client.PostAsJsonAsync("/api/veilingen", toekomstDto);

            // Create verleden veiling
            var verledenDto = new
            {
                Naam = $"Verleden {uniqueId}",
                StartTijd = now.AddHours(-5),
                EndTijd = now.AddHours(-3),
                KavelIds = new List<int>()
            };
            await _client.PostAsJsonAsync("/api/veilingen", verledenDto);

            var response = await _client.GetAsync("/api/veilingen/actief");
            var actieveVeilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            Assert.NotNull(actieveVeilingen);

            // Debug info if it fails
            if (!actieveVeilingen.Any(v => v.Id == actiefCreated.Id))
            {
                var debugInfo = $"Looking for veiling {actiefCreated.Id} with name '{actiefCreated.Naam}', " +
                                $"StartTijd: {actiefCreated.StartTijd}, EndTijd: {actiefCreated.EndTijd}, " +
                                $"Current time: {now}";
                throw new Exception(debugInfo);
            }

            // Check dat onze specifieke actieve veiling erin zit via ID
            Assert.Contains(actieveVeilingen, v => v.Id == actiefCreated.Id);
            Assert.DoesNotContain(actieveVeilingen, v => v.Naam.Contains($"Toekomst {uniqueId}"));
            Assert.DoesNotContain(actieveVeilingen, v => v.Naam.Contains($"Verleden {uniqueId}"));
        }

        [Fact]
        public async Task GetAllVeilingen_IncludesLocatieAndVeilingmeesterRelations()
        {
            // Create locatie first
            var locatie = new Locatie { Naam = "Test Loc Unique", KlokId = 99, Actief = true };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var createdLocatie = await locatieResponse.Content.ReadFromJsonAsync<Locatie>();

            // Create veiling via DTO (zonder locatie)
            var veilingDto = new
            {
                Naam = $"Relation Test Veiling {Guid.NewGuid()}",
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                KavelIds = new List<int>()
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veilingDto);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // Update veiling met locatie via PUT
            created!.LocatieId = createdLocatie!.Id;
            var updateResponse = await _client.PutAsJsonAsync($"/api/veilingen/{created.Id}", created);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Get the specific veiling with includes
            var getResponse = await _client.GetAsync($"/api/veilingen/{created.Id}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.Locatie);
            Assert.Equal("Test Loc Unique", retrieved.Locatie.Naam);
        }

        [Fact]
        public async Task GetVeiling_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/veilingen/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteVeiling_RemovesFromDatabase()
        {
            var veiling = new Models.Veiling
            {
                Naam = "To Delete",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var deleteResponse = await _client.DeleteAsync($"/api/veilingen/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/veilingen/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetVeilingenByLocatie_ReturnsCorrectVeilingen()
        {
            // Create unique locatie
            var locatie = new Locatie { Naam = $"Locatie {Guid.NewGuid()}", KlokId = 10, Actief = true };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var createdLocatie = await locatieResponse.Content.ReadFromJsonAsync<Locatie>();

            var uniqueName = $"Veiling voor locatie {Guid.NewGuid()}";

            // Create via DTO
            var veilingDto = new
            {
                Naam = uniqueName,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                KavelIds = new List<int>()
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veilingDto);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // Update met locatie
            created!.LocatieId = createdLocatie!.Id;
            await _client.PutAsJsonAsync($"/api/veilingen/{created.Id}", created);

            var response = await _client.GetAsync($"/api/veilingen/locatie/{createdLocatie.Id}");
            var veilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(veilingen);
            Assert.Contains(veilingen, v => v.Naam == uniqueName);
        }

        [Fact]
        public async Task UpdateVeiling_WithInvalidId_ReturnsNotFound()
        {
            var veiling = new Models.Veiling
            {
                Id = 99999,
                Naam = "NonExistent",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };

            var response = await _client.PutAsJsonAsync("/api/veilingen/99999", veiling);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}