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
        public async Task GetLocaties_ReturnsAllLocaties()
        {
            var response = await _client.GetAsync("/api/locaties");
            var locaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(locaties);
        }

        [Fact]
        public async Task GetActieveLocaties_ReturnsOnlyActive()
        {
            var response = await _client.GetAsync("/api/locaties/actief");
            var actieveLocaties = await response.Content.ReadFromJsonAsync<List<Locatie>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(actieveLocaties);
            // All returned locaties should be active
            Assert.All(actieveLocaties, l => Assert.True(l.Actief));
        }
    }
}