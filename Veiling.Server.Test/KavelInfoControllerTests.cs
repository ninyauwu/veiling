using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class KavelInfoControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public KavelInfoControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetKavels_WithKavelsInDatabase_ReturnsOkWithData()
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

            // Create kavel first
            var kavelDto = new
            {
                Naam = "Test Kavel",
                Description = "Test",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 50,
                Ql = "A1",
                Stadium = "Test",
                Lengte = 60.0f,
                Kleur = "FFFFFF",
                Fustcode = 123,
                AantalProductenPerContainer = 20,
                GewichtVanBloemen = 500.0f
            };
            var kavelResponse = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var createdKavel = await kavelResponse.Content.ReadFromJsonAsync<Kavel>();

            // Create veiling with kavel
            var veilingDto = new
            {
                Naam = "Test Veiling",
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                KavelIds = new List<int> { createdKavel!.Id }
            };
            var veilingResponse = await _client.PostAsJsonAsync("/api/veilingen", veilingDto);
            var createdVeiling = await veilingResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var response = await _client.GetAsync($"/api/kavelinfo/{createdVeiling!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Kavel", content);
        }

        [Fact]
        public async Task GetKavels_WithNoKavelsInDatabase_ReturnsNotFound()
        {
            // Clean database
            var existingKavels = await _client.GetAsync("/api/kavels");
            if (existingKavels.IsSuccessStatusCode)
            {
                var kavels = await existingKavels.Content.ReadFromJsonAsync<List<Kavel>>();
                if (kavels != null)
                {
                    foreach (var k in kavels)
                    {
                        await _client.DeleteAsync($"/api/kavels/{k.Id}");
                    }
                }
            }

            var response = await _client.GetAsync("/api/kavelinfo/99999");
            
            //no kavels
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task GetPendingKavels_ReturnsOnlyPendingKavels()
        {
            // Create test data
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

            var veiling = new Models.Veiling
            {
                Naam = "Test Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };
            var veilingResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var createdVeiling = await veilingResponse.Content.ReadFromJsonAsync<Models.Veiling>();

using System.Net;
using System.Net.Http.Json;
using Veiling.Server.Models;
using Xunit;

namespace Veiling.Server.Test.Controllers
{
    public class KavelInfoControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public KavelInfoControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetKavels_WithKavelsInDatabase_ReturnsOkWithData()
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

            // Create kavel first
            var kavelDto = new
            {
                Naam = "Test Kavel",
                Description = "Test",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 50,
                Ql = "A1",
                Stadium = "Test",
                Lengte = 60.0f,
                Kleur = "FFFFFF",
                Fustcode = 123,
                AantalProductenPerContainer = 20,
                GewichtVanBloemen = 500.0f
            };
            var kavelResponse = await _client.PostAsJsonAsync("/api/kavels", kavelDto);
            var createdKavel = await kavelResponse.Content.ReadFromJsonAsync<Kavel>();

            // Create veiling with kavel
            var veilingDto = new
            {
                Naam = "Test Veiling",
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                KavelIds = new List<int> { createdKavel!.Id }
            };
            var veilingResponse = await _client.PostAsJsonAsync("/api/veilingen", veilingDto);
            var createdVeiling = await veilingResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            var response = await _client.GetAsync($"/api/kavelinfo/{createdVeiling!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Kavel", content);
        }

        [Fact]
        public async Task GetKavels_WithNoKavelsInDatabase_ReturnsNotFound()
        {
            // Clean database
            var existingKavels = await _client.GetAsync("/api/kavels");
            if (existingKavels.IsSuccessStatusCode)
            {
                var kavels = await existingKavels.Content.ReadFromJsonAsync<List<Kavel>>();
                if (kavels != null)
                {
                    foreach (var k in kavels)
                    {
                        await _client.DeleteAsync($"/api/kavels/{k.Id}");
                    }
                }
            }

            var response = await _client.GetAsync("/api/kavelinfo/99999");
            
            //no kavels
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task GetPendingKavels_ReturnsOnlyPendingKavels()
        {
            // Create test data
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

            var veiling = new Models.Veiling
            {
                Naam = "Test Veiling",
                Klokduur = 5.0f,
                StartTijd = DateTime.UtcNow,
                EndTijd = DateTime.UtcNow.AddHours(2),
                GeldPerTickCode = 0.5f
            };
            var veilingResponse = await _client.PostAsJsonAsync("/api/veilingen", veiling);
            var createdVeiling = await veilingResponse.Content.ReadFromJsonAsync<Models.Veiling>();

            // Create pending kavel (Approved = null)
            var pendingKavelDto = new
            {
                Naam = "Pending Kavel",
                Description = "Wacht op goedkeuring",
                ImageUrl = "/test.jpg",
                MinimumPrijs = 10.0f,
                Aantal = 50,
                Ql = "A1",
                VeilingId = createdVeiling!.Id,
                Stadium = "Test",
                Lengte = 60.0f,
                Kleur = "FFFFFF",
                Fustcode = 123,
                AantalProductenPerContainer = 20,
                GewichtVanBloemen = 500.0f
            };
            var pendingResponse = await _client.PostAsJsonAsync("/api/kavels", pendingKavelDto);
            var pendingKavel = await pendingResponse.Content.ReadFromJsonAsync<Kavel>();

            // Get pending kavels
            var response = await _client.GetAsync("/api/kavelinfo/pending");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"kavel\"", content);
            }

            [Fact]
        public async Task GetPendingKavels_WithNoPendingKavels_ReturnsNotFound()
        {
            // Don't create any pending kavels, just call the endpoint
            var response = await _client.GetAsync("/api/kavelinfo/pending");
            
            // Should return NotFound when no pending kavels exist
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

        [Fact]
        public async Task GetPendingKavels_WithNoPendingKavels_ReturnsNotFound()
        {
            // Don't create any pending kavels, just call the endpoint
            var response = await _client.GetAsync("/api/kavelinfo/pending");
            
            // Should return NotFound when no pending kavels exist
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}