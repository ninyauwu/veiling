using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Veiling.Server.Test
{
    public class BasicTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public BasicTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Api_Root_ReturnsSuccessOrNotFound()
        {

            var response = await _client.GetAsync("/");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.NotFound,
                $"Expected OK or NotFound, but got {response.StatusCode}"
            );
        }

        [Fact]
        public async Task WeatherForecast_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/bedrijven")]
        [InlineData("/api/veilingen")]
        [InlineData("/api/kavels")]
        [InlineData("/api/locaties")]
        public async Task Api_Endpoints_ReturnSuccess(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}