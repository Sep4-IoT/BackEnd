
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json.Linq;

namespace Unit_Test_Project;


public class GreenhouseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GreenhouseIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new System.Uri("http://154.62.108.77:5047/SEP4/")
        });
    }

    [Fact]
    public async Task UpdateTemperatureAndVerify()
    {
        // Arrange
        int greenhouseId = 1;
        int expectedTemperature = 25;
        string tcpMessage = $"UPD,1,POST,TEM,{expectedTemperature}";
        string tcpAddress = "154.62.108.77";
        int tcpPort = 50000;

        // Act - Send TCP message
        await SendTcpMessage(tcpAddress, tcpPort, tcpMessage);

        // Wait for a short period to ensure the data is processed by the server
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the current temperature via RESTful API
        var response = await _client.GetAsync($"greenhouses/{greenhouseId}/current");

        // Assert - Verify the temperature in the response
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        int actualTemperature = jsonObject["Temperature"].Value<int>();

        Assert.Equal(expectedTemperature, actualTemperature);
    }

    private async Task SendTcpMessage(string address, int port, string message)
    {
        using (var client = new TcpClient(address, port))
        using (var networkStream = client.GetStream())
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            await networkStream.WriteAsync(data, 0, data.Length);
        }
    }
}
