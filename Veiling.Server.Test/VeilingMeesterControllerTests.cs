using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class VeilingmeestersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public VeilingmeestersControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateVeilingmeester_WithGebruiker_WorksCorrectly()
        {
            // create bedrijf and gebruiker first
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Veiling Organisatie",
                KVKnummer = 99999999
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Veilingmeester Jan",
                Email = "meester@veiling.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var veilingmeester = new Veilingmeester
            {
                GebruikerId = createdGebruiker!.Id,
                AantalVeilingenBeheerd = 5,
                Gebruiker = null! // Set to null, EF will load 
            };

            var response = await _client.PostAsJsonAsync("/api/veilingmeesters", veilingmeester);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"BadRequest: {error}");
            }

            var created = await response.Content.ReadFromJsonAsync<Veilingmeester>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(5, created.AantalVeilingenBeheerd);
            Assert.Equal(createdGebruiker.Id, created.GebruikerId);
        }

        [Fact]
        public async Task UpdateVeilingmeester_IncreasesAantalVeilingen()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test Organisatie",
                KVKnummer = 88888888
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Test Meester",
                Email = "test@veiling.nl",
                PhoneNumber = "0611111111",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var veilingmeester = new Veilingmeester
            {
                GebruikerId = createdGebruiker!.Id,
                AantalVeilingenBeheerd = 3,
                Gebruiker = null!
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingmeesters", veilingmeester);

            if (createResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = await createResponse.Content.ReadAsStringAsync();
                throw new Exception($"Create BadRequest: {error}");
            }

            var created = await createResponse.Content.ReadFromJsonAsync<Veilingmeester>();

            // increase aantal veilingen
            created!.AantalVeilingenBeheerd = 10;
            created.Gebruiker = null!; // Keep it null for update too

            var updateResponse = await _client.PutAsJsonAsync($"/api/veilingmeesters/{created.Id}", created);

            if (updateResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = await updateResponse.Content.ReadAsStringAsync();
                throw new Exception($"Update BadRequest: {error}");
            }

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/veilingmeesters/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Veilingmeester>();
            Assert.Equal(10, updated!.AantalVeilingenBeheerd);
        }

        [Fact]
        public async Task GetVeilingmeester_ReturnsCompleteData()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = 77777777
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Jan de Vries",
                Email = "jan@test.nl",
                PhoneNumber = "0622222222",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var veilingmeester = new Veilingmeester
            {
                GebruikerId = createdGebruiker!.Id,
                AantalVeilingenBeheerd = 7,
                Gebruiker = null!
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingmeesters", veilingmeester);
            var created = await createResponse.Content.ReadFromJsonAsync<Veilingmeester>();

            var getResponse = await _client.GetAsync($"/api/veilingmeesters/{created!.Id}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Veilingmeester>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal(7, retrieved.AantalVeilingenBeheerd);
            Assert.Equal(createdGebruiker.Id, retrieved.GebruikerId);

            Assert.NotNull(retrieved.Gebruiker);
            Assert.Equal("Jan de Vries", retrieved.Gebruiker.Name);
        }

        [Fact]
        public async Task GetAllVeilingmeesters_ReturnsList()
        {
            var bedrijf = new Bedrijf { Bedrijfsnaam = "Test", KVKnummer = Random.Shared.Next(10000000, 99999999) };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Meester 1",
                Email = $"meester1{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0611111111",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var veilingmeester = new Veilingmeester
            {
                GebruikerId = createdGebruiker!.Id,
                AantalVeilingenBeheerd = 5,
                Gebruiker = null!
            };
            await _client.PostAsJsonAsync("/api/veilingmeesters", veilingmeester);

            var response = await _client.GetAsync("/api/veilingmeesters");
            var meesters = await response.Content.ReadFromJsonAsync<List<Veilingmeester>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(meesters);
            Assert.NotEmpty(meesters);
        }

        [Fact]
        public async Task GetVeilingmeester_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/veilingmeesters/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteVeilingmeester_RemovesFromDatabase()
        {
            var bedrijf = new Bedrijf
                { Bedrijfsnaam = "Delete Test", KVKnummer = Random.Shared.Next(10000000, 99999999) };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "To Delete",
                Email = $"delete{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0699999999",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var veilingmeester = new Veilingmeester
            {
                GebruikerId = createdGebruiker!.Id,
                AantalVeilingenBeheerd = 1,
                Gebruiker = null!
            };
            var createResponse = await _client.PostAsJsonAsync("/api/veilingmeesters", veilingmeester);
            var created = await createResponse.Content.ReadFromJsonAsync<Veilingmeester>();

            var deleteResponse = await _client.DeleteAsync($"/api/veilingmeesters/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/veilingmeesters/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateVeilingmeester_WithInvalidId_ReturnsNotFound()
        {
            var veilingmeester = new Veilingmeester
            {
                Id = 99999,
                GebruikerId = "nonexistent",
                AantalVeilingenBeheerd = 1,
                Gebruiker = null!
            };

            var response = await _client.PutAsJsonAsync("/api/veilingmeesters/99999", veilingmeester);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}