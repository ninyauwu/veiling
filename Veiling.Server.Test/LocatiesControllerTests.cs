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
            // Create test locaties met unique namen
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
    
            var actief1 = new Locatie { Naam = $"Actief1_{uniqueId}", KlokId = 1, Actief = true };
            var actief2 = new Locatie { Naam = $"Actief2_{uniqueId}", KlokId = 2, Actief = true };
            var inactief = new Locatie { Naam = $"Inactief_{uniqueId}", KlokId = 3, Actief = false };
    
            var resp1 = await _client.PostAsJsonAsync("/api/locaties", actief1);
            var created1 = await resp1.Content.ReadFromJsonAsync<Locatie>();
    
            var resp2 = await _client.PostAsJsonAsync("/api/locaties", actief2);
            var created2 = await resp2.Content.ReadFromJsonAsync<Locatie>();
    
            var resp3 = await _client.PostAsJsonAsync("/api/locaties", inactief);
            var created3 = await resp3.Content.ReadFromJsonAsync<Locatie>();

            // Get alleen actieve locaties
            var response = await _client.GetAsync("/api/locaties/actief");
            var actieveLocaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveLocaties);
    
            // Check dat onze twee actieve locaties erin zitten
            Assert.Contains(actieveLocaties, l => l.Id == created1!.Id);
            Assert.Contains(actieveLocaties, l => l.Id == created2!.Id);
    
            // Check dat onze inactieve locatie er niet in zit
            Assert.DoesNotContain(actieveLocaties, l => l.Id == created3!.Id);
    
            // Alle actieve locaties moeten Actief = true hebben
            Assert.All(actieveLocaties!, l => Assert.True(l.Actief));
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
