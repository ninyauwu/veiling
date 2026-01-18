using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class BodenControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BodenControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<(Kavel kavel, Gebruiker gebruiker)> CreateTestData()
        {
            var bedrijf = new Bedrijf
            {
                Bedrijfsnaam = "Test BV",
                KVKnummer = Random.Shared.Next(10000000, 99999999)
            };
            var bedrijfResponse = await _client.PostAsJsonAsync("/api/bedrijven", bedrijf);
            var createdBedrijf = await bedrijfResponse.Content.ReadFromJsonAsync<Bedrijf>();

            var leverancier = new Leverancier
            {
                BedrijfId = createdBedrijf!.Bedrijfscode,
                IndexOfReliabilityOfInformation = "A",
                Bedrijf = null
            };
            var levResponse = await _client.PostAsJsonAsync("/api/leveranciers", leverancier);
            var createdLev = await levResponse.Content.ReadFromJsonAsync<Leverancier>();

            var kavel = new Kavel
            {
                Naam = "Test Kavel",
                Beschrijving = "Test",
                ArtikelKenmerken = "Test",
                MinimumPrijs = 10.0f,
                MaximumPrijs = 20.0f,
                Minimumhoeveelheid = 5,
                Foto = "/test.jpg",
                Kavelkleur = "FFFFFF",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 100,
                AantalProductenPerContainer = 10,
                LengteVanBloemen = 50.0f,
                GewichtVanBloemen = 400.0f,
                StageOfMaturity = "Test",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 100,
                GeldPerTickCode = "1.0",
                LeverancierId = createdLev!.Id
            };
            var kavelResponse = await _client.PostAsJsonAsync("/api/kavels", kavel);
            var createdKavel = await kavelResponse.Content.ReadFromJsonAsync<Kavel>();

            var gebruiker = new Gebruiker
            {
                Name = "Test Bieder",
                Email = $"bieder{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0612345678",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = createdBedrijf.Bedrijfscode
            };
            var gebruikerResponse = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker);
            var createdGebruiker = await gebruikerResponse.Content.ReadFromJsonAsync<Gebruiker>();

            return (createdKavel!, createdGebruiker!);
        }

        [Fact]
        public async Task CreateBod_StoresAllFieldsCorrectly()
        {
            var (kavel, gebruiker) = await CreateTestData();
            var timestamp = DateTime.Now;

            var bod = new Bod
            {
                Datumtijd = timestamp,
                HoeveelheidContainers = 25,
                Koopprijs = 17.50f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            };

            var response = await _client.PostAsJsonAsync("/api/boden", bod);
            var created = await response.Content.ReadFromJsonAsync<Bod>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            Assert.Equal(25, created.HoeveelheidContainers);
            Assert.Equal(17.50f, created.Koopprijs);
            Assert.False(created.Betaald);
            Assert.Equal(gebruiker.Id, created.GebruikerId);
            Assert.Equal(kavel.Id, created.KavelId);

            // Timestamp should be close
            Assert.True((created.Datumtijd - timestamp).TotalSeconds < 5);
        }

        [Fact]
        public async Task GetBodenByKavel_ReturnsOnlyBodenForThatKavel()
        {
            // create 2 unique kavels with different boden
            var (kavel1, gebruiker1) = await CreateTestData();
            var (kavel2, gebruiker2) = await CreateTestData();

            // Use unique prices to identify our boden
            var uniquePrice1 = 15.123f;
            var uniquePrice2 = 18.456f;
            var uniquePrice3 = 25.789f;

            // Boden voor kavel 1
            var bod1Kavel1 = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 10,
                Koopprijs = uniquePrice1,
                Betaald = false,
                GebruikerId = gebruiker1.Id,
                KavelId = kavel1.Id
            };
            await _client.PostAsJsonAsync("/api/boden", bod1Kavel1);

            var bod2Kavel1 = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 20,
                Koopprijs = uniquePrice2,
                Betaald = false,
                GebruikerId = gebruiker1.Id,
                KavelId = kavel1.Id
            };
            await _client.PostAsJsonAsync("/api/boden", bod2Kavel1);

            // Bod voor kavel 2
            var bodKavel2 = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 5,
                Koopprijs = uniquePrice3,
                Betaald = false,
                GebruikerId = gebruiker2.Id,
                KavelId = kavel2.Id
            };
            await _client.PostAsJsonAsync("/api/boden", bodKavel2);

            // get boden voor kavel 1
            var response = await _client.GetAsync($"/api/boden/kavel/{kavel1.Id}");
            var boden = await response.Content.ReadFromJsonAsync<List<Bod>>();

            // should only have boden for kavel1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(boden);
            // Check only our specific boden are there
            var ourBoden = boden.Where(b =>
                Math.Abs(b.Koopprijs - uniquePrice1) < 0.001f ||
                Math.Abs(b.Koopprijs - uniquePrice2) < 0.001f).ToList();

            Assert.Equal(2, ourBoden.Count);
            Assert.All(boden, b => Assert.Equal(kavel1.Id, b.KavelId));

            // Check the actual prices
            Assert.Contains(boden, b => Math.Abs(b.Koopprijs - uniquePrice1) < 0.001f);
            Assert.Contains(boden, b => Math.Abs(b.Koopprijs - uniquePrice2) < 0.001f);
            Assert.DoesNotContain(boden, b => Math.Abs(b.Koopprijs - uniquePrice3) < 0.001f);
        }

        [Fact]
        public async Task GetHoogsteBod_ReturnsActualHighestBid()
        {
            var (kavel, gebruiker) = await CreateTestData();

            // Create multiple boden with different prices
            var bodenData = new[]
            {
                new Bod
                {
                    Datumtijd = DateTime.Now, HoeveelheidContainers = 5, Koopprijs = 12.50f, Betaald = false,
                    GebruikerId = gebruiker.Id, KavelId = kavel.Id
                },
                new Bod
                {
                    Datumtijd = DateTime.Now, HoeveelheidContainers = 10, Koopprijs = 18.75f, Betaald = false,
                    GebruikerId = gebruiker.Id, KavelId = kavel.Id
                },
                new Bod
                {
                    Datumtijd = DateTime.Now, HoeveelheidContainers = 15, Koopprijs = 25.00f, Betaald = false,
                    GebruikerId = gebruiker.Id, KavelId = kavel.Id
                },
                new Bod
                {
                    Datumtijd = DateTime.Now, HoeveelheidContainers = 8, Koopprijs = 14.25f, Betaald = false,
                    GebruikerId = gebruiker.Id, KavelId = kavel.Id
                },
                new Bod
                {
                    Datumtijd = DateTime.Now, HoeveelheidContainers = 12, Koopprijs = 21.00f, Betaald = false,
                    GebruikerId = gebruiker.Id, KavelId = kavel.Id
                }
            };

            foreach (var bod in bodenData)
            {
                await _client.PostAsJsonAsync("/api/boden", bod);
            }

            var response = await _client.GetAsync($"/api/boden/hoogste/{kavel.Id}");
            var hoogsteBod = await response.Content.ReadFromJsonAsync<Bod>();

            // should return the highest bid
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(hoogsteBod);
            Assert.Equal(25.00f, hoogsteBod.Koopprijs);
            Assert.Equal(15, hoogsteBod.HoeveelheidContainers);
            Assert.Equal(kavel.Id, hoogsteBod.KavelId);
        }

        [Fact]
        public async Task GetBodenByGebruiker_ReturnsOnlyUsersBids()
        {
            // create 2 users
            var (kavel, gebruiker1) = await CreateTestData();

            var bedrijf2 = new Bedrijf
            {
                Bedrijfsnaam = "Bedrijf 2",
                KVKnummer = 88888888
            };
            await _client.PostAsJsonAsync("/api/bedrijven", bedrijf2);

            var gebruiker2 = new Gebruiker
            {
                Name = "User 2",
                Email = $"user2{Random.Shared.Next()}@test.nl",
                PhoneNumber = "0699999999",
                Bedrijfsbeheerder = false,
                Geverifieerd = true
            };
            var user2Response = await _client.PostAsJsonAsync("/api/gebruikers", gebruiker2);
            var createdUser2 = await user2Response.Content.ReadFromJsonAsync<Gebruiker>();

            // Create boden for user 1
            await _client.PostAsJsonAsync("/api/boden", new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 5,
                Koopprijs = 10.0f,
                Betaald = false,
                GebruikerId = gebruiker1.Id,
                KavelId = kavel.Id
            });

            await _client.PostAsJsonAsync("/api/boden", new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 10,
                Koopprijs = 15.0f,
                Betaald = false,
                GebruikerId = gebruiker1.Id,
                KavelId = kavel.Id
            });

            // Create bod for user 2
            await _client.PostAsJsonAsync("/api/boden", new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 20,
                Koopprijs = 99.0f,
                Betaald = false,
                GebruikerId = createdUser2!.Id,
                KavelId = kavel.Id
            });

            // get boden for user 1
            var response = await _client.GetAsync($"/api/boden/gebruiker/{gebruiker1.Id}");
            var boden = await response.Content.ReadFromJsonAsync<List<Bod>>();

            // should only have user1's bids
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(boden);
            Assert.Equal(2, boden.Count);
            Assert.All(boden, b => Assert.Equal(gebruiker1.Id, b.GebruikerId));

            // Check actual prices
            Assert.Contains(boden, b => b.Koopprijs == 10.0f);
            Assert.Contains(boden, b => b.Koopprijs == 15.0f);
            Assert.DoesNotContain(boden, b => b.Koopprijs == 99.0f);
        }

        [Fact]
        public async Task GetAllBoden_ReturnsAllBids()
        {
            var (kavel, gebruiker) = await CreateTestData();

            // meerdere boden
            await _client.PostAsJsonAsync("/api/boden", new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 5,
                Koopprijs = 10.0f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            });

            await _client.PostAsJsonAsync("/api/boden", new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 10,
                Koopprijs = 15.0f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            });

            var response = await _client.GetAsync("/api/boden");
            var boden = await response.Content.ReadFromJsonAsync<List<Bod>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(boden);
            Assert.True(boden.Count >= 2);
        }

        [Fact]
        public async Task GetBod_WithValidId_ReturnsBod()
        {
            var (kavel, gebruiker) = await CreateTestData();

            var bod = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 25,
                Koopprijs = 17.50f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/boden", bod);
            var created = await createResponse.Content.ReadFromJsonAsync<Bod>();

            var getResponse = await _client.GetAsync($"/api/boden/{created!.Id}");
            var retrieved = await getResponse.Content.ReadFromJsonAsync<Bod>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved.Id);
            Assert.Equal(17.50f, retrieved.Koopprijs);
            Assert.Equal(25, retrieved.HoeveelheidContainers);
        }

        [Fact]
        public async Task GetBod_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/boden/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBod_ChangesData()
        {
            var (kavel, gebruiker) = await CreateTestData();

            var bod = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 10,
                Koopprijs = 15.0f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/boden", bod);
            var created = await createResponse.Content.ReadFromJsonAsync<Bod>();

            // update
            created!.Betaald = true;
            created.Koopprijs = 20.0f;
            var updateResponse = await _client.PutAsJsonAsync($"/api/boden/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Verify changes
            var getResponse = await _client.GetAsync($"/api/boden/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<Bod>();
            Assert.True(updated!.Betaald);
            Assert.Equal(20.0f, updated.Koopprijs);
        }

        [Fact]
        public async Task DeleteBod_RemovesFromDatabase()
        {
            var (kavel, gebruiker) = await CreateTestData();

            var bod = new Bod
            {
                Datumtijd = DateTime.Now,
                HoeveelheidContainers = 5,
                Koopprijs = 10.0f,
                Betaald = false,
                GebruikerId = gebruiker.Id,
                KavelId = kavel.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/boden", bod);
            var created = await createResponse.Content.ReadFromJsonAsync<Bod>();

            // delete
            var deleteResponse = await _client.DeleteAsync($"/api/boden/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify it's gone
            var getResponse = await _client.GetAsync($"/api/boden/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetHoogsteBod_WithNoBoden_ReturnsNotFound()
        {
            // kavel zonder boden
            var (kavel, _) = await CreateTestData();

            // Act
            var response = await _client.GetAsync($"/api/boden/hoogste/{kavel.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}