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
            var now = DateTime.UtcNow;
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            var actief = new Models.Veiling
            {
                Naam = $"Actief Nu {uniqueId}",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-1),
                EndTijd = now.AddHours(1),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", actief);

            var toekomst = new Models.Veiling
            {
                Naam = $"Toekomst {uniqueId}",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(2),
                EndTijd = now.AddHours(4),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", toekomst);

            var verleden = new Models.Veiling
            {
                Naam = $"Verleden {uniqueId}",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-3),
                EndTijd = now.AddHours(-1),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", verleden);

            var response = await _client.GetAsync("/api/veilingen/actief");
            var actieveVeilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            Assert.NotNull(actieveVeilingen);
            // Check alleen dat onze actieve veiling erin zit, niet hoeveel er totaal zijn
            Assert.Contains(actieveVeilingen, v => v.Naam == $"Actief Nu {uniqueId}");
            Assert.DoesNotContain(actieveVeilingen, v => v.Naam == $"Toekomst {uniqueId}");
            Assert.DoesNotContain(actieveVeilingen, v => v.Naam == $"Verleden {uniqueId}");
        }

        [Fact]
        public async Task GetAllVeilingen_IncludesLocatieAndVeilingmeesterRelations()
        {
            // Create locatie first
            var locatie = new Locatie { Naam = "Test Loc Unique", KlokId = 99, Actief = true };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var createdLocatie = await locatieResponse.Content.ReadFromJsonAsync<Locatie>();

            var veiling = new Models.Veiling
            {
                Naam = $"Relation Test Veiling {Guid.NewGuid()}", // Unique naam
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f,
                LocatieId = createdLocatie!.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // Get all veilingen and find ours
            var response = await _client.GetAsync("/api/veilingen");
            var veilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            var retrieved = veilingen!.FirstOrDefault(v => v.Id == created!.Id);

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
            var veiling1 = new Models.Veiling
            {
                Naam = uniqueName,
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f,
                LocatieId = createdLocatie!.Id
            };
            await _client.PostAsJsonAsync("/api/veilingen", veiling1);

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