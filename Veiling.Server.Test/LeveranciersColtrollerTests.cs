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
    }
}