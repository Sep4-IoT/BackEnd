using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

public class GreenhouseIntegrationTests
{
    private readonly HttpClient _client;
    private const string TcpAddress = "154.62.108.77";
    private const int TcpPort = 50000;
    private readonly CancellationTokenSource _cts;
    private string _receivedTcpMessage;
    private readonly Task _mockTcpClientTask;
    
    public GreenhouseIntegrationTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://154.62.108.77:5047/SEP4/")
        };
        _cts = new CancellationTokenSource();
        _receivedTcpMessage = string.Empty;
        _mockTcpClientTask = Task.Run(() => StartMockTcpClient(TcpAddress, TcpPort, _cts.Token));
    }

    [Fact]
    public async Task UpdateTemperatureAndVerify()
    {
        // Arrange
        int greenhouseId = 1;
        int expectedTemperature = 27;
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
    
    // update humidity and verity:
    [Fact]
    public async Task UpdateHumidityAndVerify()
    {
        // Arrange
        int greenhouseId = 1;
        int expectedTemperature = 37;
        string tcpMessage = $"UPD,1,POST,HUM,{expectedTemperature}";
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
        int actualTemperature = jsonObject["Humidity"].Value<int>();

        Assert.Equal(expectedTemperature, actualTemperature);
    }
    
    [Fact]
    public async Task UpdateLightingAndVerify()
    {
        // Arrange
        int greenhouseId = 1;
        int expectedTemperature = 97;
        string tcpMessage = $"UPD,1,POST,LIG,{expectedTemperature}";
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
        int actualTemperature = jsonObject["LightIntensity"].Value<int>();

        Assert.Equal(expectedTemperature, actualTemperature);
    }
    
    
    [Fact]
    public async Task UpdateCO2AndVerify()
    {
        // Arrange
        int greenhouseId = 1;
        int expectedTemperature = 900;
        string tcpMessage = $"UPD,1,POST,CO2,{expectedTemperature}";
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
        int actualTemperature = jsonObject["Co2Levels"].Value<int>();

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
    
    [Fact]
    public async Task ToggleWindowStatusAndVerifyTcpMessage()
    {
        // Arrange
        int greenhouseId = 1;

        // Act - Get the current window status via RESTful API
        var response = await _client.GetAsync($"greenhouses/{greenhouseId}/current");
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);

        // Check if isWindowOpen is null and treat it as false if so
        bool isWindowOpen = jsonObject["isWindowOpen"].Type == JTokenType.Null ? false : jsonObject["isWindowOpen"].Value<bool>();

        // Toggle the window status
        bool newWindowStatus = !isWindowOpen;
        var patchContent = new StringContent($"{{ \"id\": 1, \"Name\": null, \"Description\": null, \"isWindowOpen\": " +
                                             $"{newWindowStatus.ToString().ToLower()} }}", Encoding.UTF8, "application/json");
        response = await _client.PatchAsync($"greenhouses/{greenhouseId}", patchContent);
        response.EnsureSuccessStatusCode();

        // Wait for a short period to ensure the TCP message is sent
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Assert - Verify the TCP message
        string expectedTcpMessage = newWindowStatus ? "REQ,1,SET,SER,180" : "REQ,1,SET,SER,0";
        Assert.Equal(expectedTcpMessage, _receivedTcpMessage);
    }


    private async Task StartMockTcpClient(string address, int port, CancellationToken cancellationToken)
    {
        using (TcpClient client = new TcpClient())
        {
            await client.ConnectAsync(address, port);
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        _receivedTcpMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    }
                    await Task.Delay(100, cancellationToken);
                }
            }
        }
    }
    
    [Fact]
    public async Task ToggleWindowStatusViaTcpAndVerify()
    {
        // Arrange
        int greenhouseId = 1;

        // Act - Get the current window status via RESTful API
        var response = await _client.GetAsync($"greenhouses/{greenhouseId}/current");
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        bool isWindowOpen = jsonObject["isWindowOpen"].Type == JTokenType.Null ? false : jsonObject["isWindowOpen"].Value<bool>();

        // Toggle the window status using TCP message
        string tcpMessage = isWindowOpen ? "RES,1,SER,0" : "RES,1,SER,180";
        await SendTcpMessage(TcpAddress, TcpPort, tcpMessage);

        // Wait for a short period to ensure the window status is updated
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the updated window status via RESTful API
        response = await _client.GetAsync($"greenhouses/{greenhouseId}/current");
        response.EnsureSuccessStatusCode();
        jsonResponse = await response.Content.ReadAsStringAsync();
        jsonObject = JObject.Parse(jsonResponse);
        bool newWindowStatus = jsonObject["isWindowOpen"].Value<bool>();

        // Assert - Verify the window status has been toggled
        Assert.Equal(!isWindowOpen, newWindowStatus);
    }
    
    [Fact]
    public async Task SendHighTemperatureAndVerifyResponse()
    {
        // Arrange
        int greenhouseId = 1;
        int temperature = 45;
        string tcpMessage = $"UPD,1,POST,TEM,{temperature}";

        // Act - Send TCP message
        await SendTcpMessage(TcpAddress, TcpPort, tcpMessage);

        // Wait for a short period to ensure the message is processed
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Assert - Verify the TCP message
        string expectedTcpMessage = "REQ,1,SET,SER,180";
        Assert.Equal(expectedTcpMessage, _receivedTcpMessage);
    }
    
    [Fact]
    public async Task LoginAndVerifyToken()
    {
        // Arrange
        var loginData = new
        {
            username = "admin",
            password = "via"
        };
        string jsonLoginData = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
        var content = new StringContent(jsonLoginData, Encoding.UTF8, "application/json");

        // Act - Send POST request to login
        var response = await _client.PostAsync("login", content);
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);

        // Assert - Verify the response contains a token
        Assert.True(jsonObject.ContainsKey("token"), "Response does not contain 'token' key.");
        string token = jsonObject["token"].Value<string>();
        Assert.NotNull(token);
        Assert.Equal(180, token.Length);
    }
    
    [Fact]
    public async Task GetGreenhouseHistoryAndVerifyResponse()
    {
        // Act - Send GET request to history endpoint
        var response = await _client.GetAsync("greenhouses/1/history");
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonArray = JArray.Parse(jsonResponse);

        // Assert - Verify the response format and contents
        Assert.NotNull(jsonArray);
        Assert.True(jsonArray.Count > 0, "The response should contain at least one greenhouse status entry.");

        foreach (var jsonObject in jsonArray)
        {
            Assert.NotNull(jsonObject["Id"]);
            Assert.NotNull(jsonObject["Name"]);
            Assert.NotNull(jsonObject["Description"]);

            // Verify optional fields
            Assert.True(jsonObject["Temperature"] == null || jsonObject["Temperature"].Type != JTokenType.String);
            Assert.True(jsonObject["LightIntensity"] == null || jsonObject["LightIntensity"].Type != JTokenType.String);
            Assert.True(jsonObject["Co2Levels"] == null || jsonObject["Co2Levels"].Type != JTokenType.String);
            Assert.True(jsonObject["Humidity"] == null || jsonObject["Humidity"].Type != JTokenType.String);
            Assert.True(jsonObject["isWindowOpen"] == null || jsonObject["isWindowOpen"].Type == JTokenType.Boolean);
            Assert.True(jsonObject["date"] == null || jsonObject["date"].Type == JTokenType.Date);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _mockTcpClientTask.Wait();
    }
}