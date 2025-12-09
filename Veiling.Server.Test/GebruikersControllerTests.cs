using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class GebruikersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public GebruikersControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateGebruiker_WithBedrijf_WorksCorrectly()
        {
            // create bedrijf first
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test Bedrijf",
                KVKnummer = 12345678
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Jan de Vries",
                Email = "jan@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = true,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };

            var response = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var created = await response.Content.ReadFromJsonAsync<Gebruiker>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.NotNull(created.Id);
            Assert.Equal("Jan de Vries", created.Name);
            Assert.Equal(createdBedrijf.Bedrijfscode, created.BedrijfId);
            Assert.True(created.Bedrijfsbeheerder);
        }

        [Fact]
        public async Task GetGebruikersByBedrijf_ReturnsCorrectUsers()
        {
            // create bedrijf and users
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Multi User Bedrijf",
                KVKnummer = 11111111
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruikers = new[]
            {
                new Gebruiker { Name = "User 1", Email = "user1@test.nl", PhoneNumber = "0611111111", Bedrijfsbeheerder = true, Geverifieerd = true, BedrijfId = createdBedrijf!.Bedrijfscode },
                new Gebruiker { Name = "User 2", Email = "user2@test.nl", PhoneNumber = "0622222222", Bedrijfsbeheerder = false, Geverifieerd = true, BedrijfId = createdBedrijf.Bedrijfscode }
            };

            foreach (var g in gebruikers)
            {
                await _client.PostAsJsonAsync("/api/gebruikers", g);
            }

            var response = await _client.GetAsync($"/api/gebruikers/bedrijf/{createdBedrijf.Bedrijfscode}");
            var result = await response.Content.ReadFromJsonAsync<List<Gebruiker>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result.Count >= 2);
            Assert.Contains(result, u => u.Name == "User 1");
            Assert.Contains(result, u => u.Name == "User 2");
        }
    }
}