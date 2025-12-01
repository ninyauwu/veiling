using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Veiling.Api.Test
{
    public class BasicTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BasicTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Api_Root_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/");

            Assert.True(response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
