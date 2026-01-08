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
            // create locatie first
            var locatie = new Locatie
            {
                Naam = "Amsterdam",
                KlokId = 1,
                Actief = true
            };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);

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
            // create actieve veiling
            var veiling = new Models.Veiling
            {
                Naam = "Test Actieve Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow.AddHours(-1), // Started 1 hour ago
                EndTijd = DateTime.UtcNow.AddHours(2), // Ends in 2 hours
                GeldPerTickCode = 0.5f
            };

            await _client.PostAsJsonAsync("/api/veilingen", veiling);

            var response = await _client.GetAsync("/api/veilingen/actief");
            var actieveVeilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveVeilingen);
            // Should have at least our created veiling
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

            // extend end time
            var newEndTime = DateTime.UtcNow.AddHours(4);
            created!.EndTijd = newEndTime;
            var updateResponse = await _client.PutAsJsonAsync($"/api/veilingen/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/veilingen/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // compare
            Assert.True(Math.Abs((updated!.EndTijd - newEndTime).TotalMinutes) < 1);
        }

        [Fact]
        public async Task CreateVeiling_WithEndTimeBeforeStartTime_ReturnsBadRequest()
        {
            // eindtijd moet na starttijd zijn
            var veiling = new Models.Veiling
            {
                Naam = "Invalid Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow.AddHours(2),
                EndTijd = DateTime.UtcNow.AddHours(1),
                GeldPerTickCode = 0.5f
            };

            var response = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetActieveVeilingen_OnlyReturnsCurrentlyActiveOnes()
        {
            // Test dat de time-based filtering werkt
            var now = DateTime.UtcNow;

            // Actieve veiling
            var actief = new Models.Veiling
            {
                Naam = "Actief Nu",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-1),
                EndTijd = now.AddHours(1),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", actief);

            // Toekomstige veiling
            var toekomst = new Models.Veiling
            {
                Naam = "Toekomst",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(2),
                EndTijd = now.AddHours(4),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", toekomst);

            // Afgelopen veiling
            var verleden = new Models.Veiling
            {
                Naam = "Verleden",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-3),
                EndTijd = now.AddHours(-1),
                GeldPerTickCode = 0.5f
            };
            await _client.PostAsJsonAsync("/api/veilingen", verleden);

            var response = await _client.GetAsync("/api/veilingen/actief");
            var actieveVeilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            // Alleen "Actief Nu" zou terug moeten komen
            Assert.NotNull(actieveVeilingen);
            Assert.Single(actieveVeilingen);
            Assert.Equal("Actief Nu", actieveVeilingen[0].Naam);
        }

        [Fact]
        public async Task UpdateVeiling_ToInvalidTimeRange_ReturnsBadRequest()
        {
            // Maak geldige veiling
            var veiling = new Models.Veiling
            {
                Naam = "Test",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // Update naar ongeldige tijden
            created!.EndTijd = created.StartTijd.AddHours(-1);
            var updateResponse = await _client.PutAsJsonAsync($"/api/veilingen/{created.Id}", created);

            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllVeilingen_IncludesLocatieAndVeilingmeesterRelations()
        {
            var locatie = new Locatie { Naam = "Test Loc", KlokId = 99, Actief = true };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var createdLocatie = await locatieResponse.Content.ReadFromJsonAsync<Locatie>();

            var veiling = new Models.Veiling
            {
                Naam = "Relation Test Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f,
                LocatieId = createdLocatie!.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var created = await createResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var response = await _client.GetAsync("/api/veilingen");
            var veilingen = await response.Content.ReadFromJsonAsync<List<Models.Veiling>>();

            var retrieved = veilingen!.FirstOrDefault(v => v.Id == created!.Id);

            // CRITICAL: Test relations are actually loaded
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.Locatie);
            Assert.Equal("Test Loc", retrieved.Locatie.Naam);
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
            var locatie = new Locatie { Naam = "Test Locatie", KlokId = 10, Actief = true };
            var locatieResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var createdLocatie = await locatieResponse.Content.ReadFromJsonAsync<Locatie>();

            var veiling1 = new Models.Veiling
            {
                Naam = "Veiling voor locatie",
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
            Assert.Contains(veilingen, v => v.Naam == "Veiling voor locatie");
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