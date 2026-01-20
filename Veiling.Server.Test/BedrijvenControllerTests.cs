using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class BedrijvenControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BedrijvenControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateBedrijf_StoresAllFieldsCorrectly()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Bloemen Express BV",
                KVKnummer = 12345678
            };
            
            var response = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var created = await response.Content.ReadFromJsonAsync<Bedrijf>();
            
            // check all fields
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Bedrijfscode > 0, "Bedrijfscode should be auto-generated");
            Assert.Equal("Bloemen Express BV", created.Bedrijfsnaam);
            Assert.Equal(12345678, created.KVKnummer);
        }

        [Fact]
        public async Task GetBedrijf_ReturnsExactSameData()
        {
            // create bedrijf
            var original = new Bedrijf
            {
                Bedrijfsnaam = "Test Bedrijf XYZ",
                KVKnummer = 99887766
            };
            var createResponse = await _client.PostAsJsonAsync("/api/bedrijven", original);
            var created = await createResponse.Content.ReadFromJsonAsync<Bedrijf>();
            
            var getResponse = await _client.GetAsync($"/api/bedrijven/{created!.Bedrijfscode}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Bedrijf>();

            // verify data
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Bedrijfscode, retrieved.Bedrijfscode);
            Assert.Equal("Test Bedrijf XYZ", retrieved.Bedrijfsnaam);
            Assert.Equal(99887766, retrieved.KVKnummer);
        }

        [Fact]
        public async Task UpdateBedrijf_ChangesDataInDatabase()
        {
            // create original
            var original = new Bedrijf
            {
                Bedrijfsnaam = "Original Name",
                KVKnummer = 11111111
            };
            var createResponse = await _client.PostAsJsonAsync("/api/bedrijven", original);
            var created = await createResponse.Content.ReadFromJsonAsync<Bedrijf>();

            // update both fields
            created!.Bedrijfsnaam = "UPDATED NAME";
            created.KVKnummer = 99999999;
            var updateResponse = await _client.PutAsJsonAsync($"/api/bedrijven/{created.Bedrijfscode}", created);

            // verify database changed
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
            
            var verifyResponse = await _client.GetAsync($"/api/bedrijven/{created.Bedrijfscode}");
            var updated = await verifyResponse.Content.ReadFromJsonAsync<Bedrijf>();
            
            Assert.NotNull(updated);
            Assert.Equal("UPDATED NAME", updated.Bedrijfsnaam);
            Assert.Equal(99999999, updated.KVKnummer);
            Assert.Equal(created.Bedrijfscode, updated.Bedrijfscode);
        }

        [Fact]
        public async Task GetAllBedrijven_ReturnsAllWithCorrectData()
        {
            // create multiple bedrivjen
            var testData = new[]
            {
                new Bedrijf { Bedrijfsnaam = "Alpha Bloemen", KVKnummer = 10000001 },
                new Bedrijf { Bedrijfsnaam = "Beta Flora", KVKnummer = 20000002 },
                new Bedrijf { Bedrijfsnaam = "Gamma Planten", KVKnummer = 30000003 }
            };

            foreach (var b in testData)
            {
                await _client.PostAsJsonAsync("/api/bedrijven", b);
            }

            var response = await _client.GetAsync("/api/bedrijven");
            var result = await response.Content.ReadFromJsonAsync<List<Bedrijf>>();

            // check data
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result.Count >= 3);
            
            // Verify each bedrijf
            var alpha = result.FirstOrDefault(b => b.Bedrijfsnaam == "Alpha Bloemen");
            Assert.NotNull(alpha);
            Assert.Equal(10000001, alpha.KVKnummer);
            
            var beta = result.FirstOrDefault(b => b.Bedrijfsnaam == "Beta Flora");
            Assert.NotNull(beta);
            Assert.Equal(20000002, beta.KVKnummer);
            
            var gamma = result.FirstOrDefault(b => b.Bedrijfsnaam == "Gamma Planten");
            Assert.NotNull(gamma);
            Assert.Equal(30000003, gamma.KVKnummer);
        }

        [Fact]
        public async Task DeleteBedrijf_ActuallyRemovesFromDatabase()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "To Be Deleted",
                KVKnummer = 88888888
            };
            var createResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var created = await createResponse.Content.ReadFromJsonAsync<Bedrijf>();
            
            var existsResponse = await _client.GetAsync($"/api/bedrijven/{created!.Bedrijfscode}");
            Assert.Equal(HttpStatusCode.OK, existsResponse.StatusCode);

            // delete it
            var deleteResponse = await _client.DeleteAsync($"/api/bedrijven/{created.Bedrijfscode}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // verify it's REALLY gone from database
            var notFoundResponse = await _client.GetAsync($"/api/bedrijven/{created.Bedrijfscode}");
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
            
            // Also check it's not in the list anymore
            var allResponse = await _client.GetAsync("/api/bedrijven");
            var all = await allResponse.Content.ReadFromJsonAsync<List<Bedrijf>>();
            Assert.DoesNotContain(all!, b => b.Bedrijfscode == created.Bedrijfscode);
        }
    }
}
