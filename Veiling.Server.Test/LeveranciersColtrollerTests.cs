using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class LeveranciersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public LeveranciersControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateLeverancier_WithBedrijf_WorksCorrectly()
        {
            //create bedrijf first
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Bloemen Leverancier BV",
                KVKnummer = 12345678
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
            var created = await response.Content.ReadFromJsonAsync<Leverancier>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("A", created.IndexOfReliabilityOfInformation);
            Assert.Equal(createdBedrijf.Bedrijfscode, created.BedrijfId);
        }

        [Fact]
        public async Task UpdateLeverancier_ChangesReliabilityIndex()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = 87654321
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var leverancier = new Leverancier
            {
                BedrijfId = createdBedrijf!.Bedrijfscode,
                IndexOfReliabilityOfInformation = "A",
                Bedrijf = null
            };
            var createResponse = await _client.PostAsJsonAsync("/api/leveranciers", leverancier);
            var created = await createResponse.Content.ReadFromJsonAsync<Leverancier>();

            // update index
            created!.IndexOfReliabilityOfInformation = "B";
            var updateResponse = await _client.PutAsJsonAsync($"/api/leveranciers/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/leveranciers/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Leverancier>();
            Assert.Equal("B", updated!.IndexOfReliabilityOfInformation);
        }

        [Fact]
        public async Task GetAllLeveranciers_IncludesBedrijfRelation()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Unique Test BV",
                KVKnummer = 88888888
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var leverancier = new Leverancier
            {
                BedrijfId = createdBedrijf!.Bedrijfscode,
                IndexOfReliabilityOfInformation = "A",
                Bedrijf = null
            };
            var createResponse = await _client.PostAsJsonAsync("/api/leveranciers", leverancier);
            var created = await createResponse.Content.ReadFromJsonAsync<Leverancier>();

            // Get all leveranciers
            var response = await _client.GetAsync("/api/leveranciers");
            var leveranciers = await response.Content.ReadFromJsonAsync<List<Leverancier>>();

            // Find our specific leverancier
            var retrieved = leveranciers!.FirstOrDefault(l => l.Id == created!.Id);

            // Verify Bedrijf relation is loaded
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.Bedrijf);
            Assert.Equal("Unique Test BV", retrieved.Bedrijf.Bedrijfsnaam);
            Assert.Equal(88888888, retrieved.Bedrijf.KVKnummer);
        }

        [Fact]
        public async Task GetLeverancier_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/leveranciers/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteLeverancier_RemovesFromDatabase()
        {
            var bedrijf = new Bedrijf { Bedrijfsnaam = "Delete Test", KVKnummer = 99999999 };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var leverancier = new Leverancier
            {
                BedrijfId = createdBedrijf!.Bedrijfscode,
                IndexOfReliabilityOfInformation = "C",
                Bedrijf = null
            };
            var createResponse = await _client.PostAsJsonAsync("/api/leveranciers", leverancier);
            var created = await createResponse.Content.ReadFromJsonAsync<Leverancier>();

            var deleteResponse = await _client.DeleteAsync($"/api/leveranciers/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/leveranciers/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}