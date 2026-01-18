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
            // Create een test locatie eerst
            var locatie = new Locatie { Naam = "Test Locatie", KlokId = 1, Actief = true };
            await _client.PostAsJsonAsync("/api/locaties", locatie);
            
            var response = await _client.GetAsync("/api/locaties");
            var locaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(locaties);
            Assert.NotEmpty(locaties);
            Assert.All(locaties, l => Assert.True(l.Id > 0));
        }

        [Fact]
        public async Task GetActieveLocaties_ReturnsOnlyActive_AndIsSubsetOfAll()
        {
            // Create test locaties
            await _client.PostAsJsonAsync("/api/locaties", new Locatie { Naam = "Actief", KlokId = 1, Actief = true });
            await _client.PostAsJsonAsync("/api/locaties", new Locatie { Naam = "Inactief", KlokId = 2, Actief = false });
            
            var allResponse = await _client.GetAsync("/api/locaties");
            var allLocaties = await allResponse.Content.ReadFromJsonAsync<List<Locatie>>();
            Assert.NotNull(allLocaties);

            var response = await _client.GetAsync("/api/locaties/actief");
            var actieveLocaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveLocaties);
            Assert.All(actieveLocaties!, l => Assert.True(l.Actief));

            var allIds = allLocaties!.Select(l => l.Id).ToHashSet();
            Assert.All(actieveLocaties!, l => Assert.Contains(l.Id, allIds));
        }

        [Fact]
        public async Task GetLocatie_WithExistingId_ReturnsSingleLocatie()
        {
            // Create locatie eerst
            var locatie = new Locatie { Naam = "Test Locatie", KlokId = 10, Actief = true };
            var createResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var created = await createResponse.Content.ReadFromJsonAsync<Locatie>();

            var response = await _client.GetAsync($"/api/locaties/{created!.Id}");
            var retrieved = await response.Content.ReadFromJsonAsync<Locatie>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved!.Id);
        }


        [Fact]
        public async Task GetLocatie_WithNonExistingId_ReturnsNotFound()
        {
            // kies een Id die zeker niet bestaat
            const int nonExistingId = int.MaxValue;

            var response = await _client.GetAsync($"/api/locaties/{nonExistingId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLocatie_WithValidData_UpdatesAndReturnsNoContent()
        {
            // Create locatie eerst
            var locatie = new Locatie { Naam = "Original", KlokId = 5, Actief = true };
            var createResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var created = await createResponse.Content.ReadFromJsonAsync<Locatie>();

            var originalActief = created!.Actief;
            created.Actief = !created.Actief; 

            var putResponse = await _client.PutAsJsonAsync($"/api/locaties/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/locaties/{created.Id}");
            var updatedLocatie = await getResponse.Content.ReadFromJsonAsync<Locatie>();

            Assert.NotNull(updatedLocatie);
            Assert.Equal(created.Id, updatedLocatie!.Id);
            Assert.Equal(!originalActief, updatedLocatie.Actief);
        }

        [Fact]
        public async Task UpdateLocatie_WithIdMismatch_ReturnsBadRequest()
        {
            // Create locatie eerst
            var locatie = new Locatie { Naam = "Test", KlokId = 1, Actief = true };
            var createResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var created = await createResponse.Content.ReadFromJsonAsync<Locatie>();

            var routeId = created!.Id + 1;

            var response = await _client.PutAsJsonAsync($"/api/locaties/{routeId}", created);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task CreateLocatie_WithValidData_ReturnsCreated()
        {
            var locatie = new Locatie {
                Naam = "Utrecht",
                KlokId = 4,
                Actief = true
            };

            var response = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var created = await response.Content.ReadFromJsonAsync<Locatie>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Utrecht", created.Naam);
            Assert.Equal(4, created.KlokId);
            Assert.True(created.Actief);
        }

        [Fact]
        public async Task DeleteLocatie_WithValidId_RemovesFromDatabase()
        {
            var locatie = new Locatie {
                Naam = "To Delete",
                KlokId = 5,
                Actief = false
            };
            var createResponse = await _client.PostAsJsonAsync("/api/locaties", locatie);
            var created = await createResponse.Content.ReadFromJsonAsync<Locatie>();

            var deleteResponse = await _client.DeleteAsync($"/api/locaties/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/locaties/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateLocatie_WithNonExistingId_ReturnsNotFound()
        {
            var locatie = new Locatie {
                Id = 99999,
                Naam = "NonExistent",
                KlokId = 1,
                Actief = true
            };

            var response = await _client.PutAsJsonAsync("/api/locaties/99999", locatie);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
