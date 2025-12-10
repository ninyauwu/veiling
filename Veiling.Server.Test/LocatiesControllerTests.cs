using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class LocatiesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public LocatiesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetLocaties_ReturnsAllLocaties_WithAtLeastOneItem()
        {
            // Act
            var response = await _client.GetAsync("/api/locaties");
            var locaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(locaties);
            Assert.NotEmpty(locaties);                // we verwachten testdata
            Assert.All(locaties, l => Assert.True(l.Id > 0)); // basic sanity check
        }

        [Fact]
        public async Task GetActieveLocaties_ReturnsOnlyActive_AndIsSubsetOfAll()
        {
            // Arrange: haal alle locaties op
            var allResponse = await _client.GetAsync("/api/locaties");
            var allLocaties = await allResponse.Content.ReadFromJsonAsync<List<Locatie>>();
            Assert.NotNull(allLocaties);

            // Act: haal alleen actieve locaties op
            var response = await _client.GetAsync("/api/locaties/actief");
            var actieveLocaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveLocaties);
            Assert.All(actieveLocaties!, l => Assert.True(l.Actief));

            // Alle actieve locaties moeten subset zijn van allLocaties
            var allIds = allLocaties!.Select(l => l.Id).ToHashSet();
            Assert.All(actieveLocaties!, l => Assert.Contains(l.Id, allIds));
        }

        [Fact]
        public async Task GetLocatie_WithExistingId_ReturnsSingleLocatie()
        {
            // Arrange: pak eerst een geldige Id uit de lijst
            var allResponse = await _client.GetAsync("/api/locaties");
            var allLocaties = await allResponse.Content.ReadFromJsonAsync<List<Locatie>>();
            Assert.NotNull(allLocaties);
            var first = allLocaties!.First();

            // Act
            var response = await _client.GetAsync($"/api/locaties/{first.Id}");
            var locatie = await response.Content.ReadFromJsonAsync<Locatie>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(locatie);
            Assert.Equal(first.Id, locatie!.Id);
        }

        [Fact]
        public async Task GetLocatie_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange: kies een Id die zeker niet bestaat
            const int nonExistingId = int.MaxValue;

            // Act
            var response = await _client.GetAsync($"/api/locaties/{nonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLocatie_WithValidData_UpdatesAndReturnsNoContent()
        {
            // Arrange: haal bestaande locatie op
            var allResponse = await _client.GetAsync("/api/locaties");
            var allLocaties = await allResponse.Content.ReadFromJsonAsync<List<Locatie>>();
            Assert.NotNull(allLocaties);
            var locatie = allLocaties!.First();

            var originalActief = locatie.Actief;
            locatie.Actief = !locatie.Actief; 

            // Act: stuur PUT met hetzelfde Id
            var putResponse = await _client.PutAsJsonAsync($"/api/locaties/{locatie.Id}", locatie);

            // Assert response
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Assert dat het ook echt is opgeslagen door opnieuw te fetchen
            var getResponse = await _client.GetAsync($"/api/locaties/{locatie.Id}");
            var updatedLocatie = await getResponse.Content.ReadFromJsonAsync<Locatie>();

            Assert.NotNull(updatedLocatie);
            Assert.Equal(locatie.Id, updatedLocatie!.Id);
            Assert.Equal(!originalActief, updatedLocatie.Actief);
        }

        [Fact]
        public async Task UpdateLocatie_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange: haal bestaande locatie op
            var allResponse = await _client.GetAsync("/api/locaties");
            var allLocaties = await allResponse.Content.ReadFromJsonAsync<List<Locatie>>();
            Assert.NotNull(allLocaties);
            var locatie = allLocaties!.First();

            var routeId = locatie.Id + 1;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/locaties/{routeId}", locatie);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
