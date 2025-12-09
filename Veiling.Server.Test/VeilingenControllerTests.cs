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
                EndTijd = DateTime.UtcNow.AddHours(2),    // Ends in 2 hours
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
    }
}