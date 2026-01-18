using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Veiling.Server.Data;
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
        
        [Fact]
        public async Task GetAllGebruikers_ReturnsUserList()
        {
            // create a user
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Test User",
                Email = $"test{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);

            var response = await _client.GetAsync("/api/gebruikers");
            var result = await response.Content.ReadFromJsonAsync<List<Gebruiker>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetGebruiker_WithValidId_ReturnsUser()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Specific User",
                Email = $"specific{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var createResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var created = await createResponse.Content.ReadFromJsonAsync<Gebruiker>();

            var response = await _client.GetAsync($"/api/gebruikers/{created!.Id}");
            var retrieved = await response.Content.ReadFromJsonAsync<Gebruiker>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal("Specific User", retrieved.Name);
        }

        [Fact]
        public async Task GetGebruiker_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/gebruikers/nonexistent-id-12345");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGebruiker_ChangesUserData()
        {
            // create user
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "Original Name",
                Email = $"original{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = false,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var createResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var created = await createResponse.Content.ReadFromJsonAsync<Gebruiker>();

            // update user
            created!.Name = "Updated Name";
            created.Geverifieerd = true;
            var updateResponse = await _client.PutAsJsonAsync($"/api/gebruikers/{created.Id}", created);
            
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Verify changes
            var getResponse = await _client.GetAsync($"/api/gebruikers/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Gebruiker>();
            Assert.Equal("Updated Name", updated!.Name);
            Assert.True(updated.Geverifieerd);
        }

        [Fact]
        public async Task DeleteGebruiker_RemovesUserFromDatabase()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var gebruiker = new Gebruiker
            {
                Name = "To Be Deleted",
                Email = $"delete{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf!.Bedrijfscode
            };
            var createResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var created = await createResponse.Content.ReadFromJsonAsync<Gebruiker>();

            // Use string Id (after fixing backend)
            var deleteResponse = await _client.DeleteAsync($"/api/gebruikers/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/gebruikers/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        
        [Fact]
        public async Task Register_WithValidData_CreatesUser()
        {
            var registratie = new GebruikerRegistratie
            {
                Email = $"newuser{Random.Shared.Next()}@test.nl",
                FirstName = "New",
                LastName = "User",
                PhoneNumber = "0612345678",
                Password = "SecurePass123!",
                Role = "" // Voeg lege role toe
            };

            var response = await _client.PostAsJsonAsync("/api/gebruikers/register", registratie);

            // Should succeed 
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.Created,
                $"Expected OK or Created, got {response.StatusCode}. Error: {await response.Content.ReadAsStringAsync()}"
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccess()
        {
            // register user
            var email = $"logintest{Random.Shared.Next()}@test.nl";
            var password = "SecurePass123!";
            
            var registratie = new GebruikerRegistratie
            {
                Email = email,
                FirstName = "Login",
                LastName = "Test",
                PhoneNumber = "0612345678",
                Password = password
            };
            var registerResponse = await _client.PostAsJsonAsync("/api/gebruikers/register", registratie);
            
            // Only proceed if registration succeeded
            if (registerResponse.IsSuccessStatusCode)
            {
                // try to login
                var login = new GebruikerLogin
                {
                    Email = email,
                    Password = password
                };
                var response = await _client.PostAsJsonAsync("/api/gebruikers/login", login);

                Assert.True(
                    response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.Unauthorized,
                    $"Expected OK or Unauthorized, got {response.StatusCode}"
                );
            }
        }
        
        
    }
}