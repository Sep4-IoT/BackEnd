using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

public class GreenhouseIntegrationTests
{
    private readonly HttpClient _client;
    private const string TcpAddress = "localhost";
    private const int TcpPort = 50000;
    private readonly CancellationTokenSource _cts;
    private string _receivedTcpMessage;
    private readonly Task _mockTcpClientTask;
    private string _token;
    private readonly ITestOutputHelper _output;
    private Random _random;

    public GreenhouseIntegrationTests(ITestOutputHelper output)
    {
        _random = new Random();
        _output = output;
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5047/SEP4/")
        };
        _cts = new CancellationTokenSource();
        _receivedTcpMessage = string.Empty;
        _mockTcpClientTask = Task.Run(() => StartMockTcpClient(TcpAddress, TcpPort, _cts.Token));
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
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
        _token = jsonObject["token"].Value<string>();

        Assert.NotNull(_token);
        Assert.Equal(180, _token.Length);

        _output.WriteLine($"Token: {_token}");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task UpdateTemperatureAndVerify(int greenhouseId)
    {
        // Arrange
        int expectedTemperature = _random.Next(1, 40);
        string tcpMessage = $"UPD,{greenhouseId},POST,TEM,{expectedTemperature}";
        string tcpAddress = "localhost";
        int tcpPort = 50000;
        

        // Act - Send TCP message
        await SendTcpMessage(tcpAddress, tcpPort, tcpMessage);

        // Wait for a short period to ensure the data is processed by the server
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the current temperature via RESTful API
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);

        // Assert - Verify the temperature in the response
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        int actualTemperature = jsonObject["Temperature"].Value<int>();

        Assert.Equal(expectedTemperature, actualTemperature);
    }

// update humidity and verity:
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task UpdateHumidityAndVerify(int greenhouseId)
    {
        int expectedHumidity = _random.Next(1, 100);
        string tcpMessage = $"UPD,{greenhouseId},POST,HUM,{expectedHumidity}";
        string tcpAddress = "localhost";
        int tcpPort = 50000;

        // Act - Send TCP message
        await SendTcpMessage(tcpAddress, tcpPort, tcpMessage);

        // Wait for a short period to ensure the data is processed by the server
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the current temperature via RESTful API
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);

        // Assert - Verify the temperature in the response
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        int actualHumidity = jsonObject["Humidity"].Value<int>();

        Assert.Equal(expectedHumidity, actualHumidity);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task UpdateLightingAndVerify(int greenhouseId)
    {
        // Arrange
        int expectedLighting = _random.Next(1, 500);
        string tcpMessage = $"UPD,{greenhouseId},POST,LIG,{expectedLighting}";
        string tcpAddress = "localhost";
        int tcpPort = 50000;

        // Act - Send TCP message
        await SendTcpMessage(tcpAddress, tcpPort, tcpMessage);

        // Wait for a short period to ensure the data is processed by the server
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the current temperature via RESTful API
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);

        // Assert - Verify the temperature in the response
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        int actualLighting = jsonObject["LightIntensity"].Value<int>();

        Assert.Equal(expectedLighting, actualLighting);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task UpdateCO2AndVerify(int greenhouseId)
    {
        // Arrange
        int expectedCO2 = _random.Next(400, 2000);
        string tcpMessage = $"UPD,{greenhouseId},POST,CO2,{expectedCO2}";
        string tcpAddress = "localhost";
        int tcpPort = 50000;

        // Act - Send TCP message
        await SendTcpMessage(tcpAddress, tcpPort, tcpMessage);

        // Wait for a short period to ensure the data is processed by the server
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the current temperature via RESTful API
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);

        // Assert - Verify the temperature in the response
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        int actualCO2 = jsonObject["Co2Levels"].Value<int>();

        Assert.Equal(expectedCO2, actualCO2);
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

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task ToggleWindowStatusAndVerifyTcpMessage(int greenhouseId)
    {
        // Act - Send GET request to current endpoint
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var getResponse = await _client.SendAsync(getRequest);
        getResponse.EnsureSuccessStatusCode();
        string jsonResponse = await getResponse.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);

        // Check if isWindowOpen is null and treat it as false if so
        bool isWindowOpen = jsonObject["isWindowOpen"].Type == JTokenType.Null
            ? false
            : jsonObject["isWindowOpen"].Value<bool>();
        
        _output.WriteLine($"{isWindowOpen}");

        // Toggle the window status
        bool newWindowStatus = !isWindowOpen;
        var patchContent = new StringContent(
            $"{{ \"id\": 1, \"Name\": null, \"Description\": null, \"isWindowOpen\": {newWindowStatus.ToString().ToLower()} }}", 
            Encoding.UTF8, 
            "application/json"
        );

        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"greenhouses/{greenhouseId}");
        patchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        patchRequest.Content = patchContent;
        _output.WriteLine($"{patchRequest}");
        /*var patchResponse = */await _client.SendAsync(patchRequest);
        //patchResponse.EnsureSuccessStatusCode();

        // Wait for a short period to ensure the TCP message is sent
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Assert - Verify the TCP message
        string expectedTcpMessage = newWindowStatus ? $"REQ,{greenhouseId},SET,SER,180" : $"REQ,{greenhouseId},SET,SER,0";
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

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task ToggleWindowStatusViaTcpAndVerify(int greenhouseId)
    {
        // Arrange

        // Act - Get the current window status via RESTful API
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonResponse);
        bool isWindowOpen = jsonObject["isWindowOpen"].Type == JTokenType.Null
            ? false
            : jsonObject["isWindowOpen"].Value<bool>();

        // Toggle the window status using TCP message
        string tcpMessage = isWindowOpen ? $"RES,{greenhouseId},SER,0" : $"RES,{greenhouseId},SER,180";
        await SendTcpMessage(TcpAddress, TcpPort, tcpMessage);

        // Wait for a short period to ensure the window status is updated
        await Task.Delay(2000); // Adjust delay as necessary for the system to process

        // Act - Get the updated window status via RESTful API
        var request2 = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/current");
        request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        response = await _client.SendAsync(request2);
        response.EnsureSuccessStatusCode();
        jsonResponse = await response.Content.ReadAsStringAsync();
        jsonObject = JObject.Parse(jsonResponse);
        bool newWindowStatus = jsonObject["isWindowOpen"].Value<bool>();

        // Assert - Verify the window status has been toggled
        Assert.Equal(!isWindowOpen, newWindowStatus);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task SendHighTemperatureAndVerifyResponse(int greenhouseId)
    {
        int temperature = _random.Next(41, 70);
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
        _token = jsonObject["token"].Value<string>();
        
        Assert.NotNull(_token);
        Assert.Equal(180, _token.Length);
    }

    /*[Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task GetGreenhouseHistoryAndVerifyResponse(int greenhouseId)
    {
        // Act - Send GET request to history endpoint
        var request = new HttpRequestMessage(HttpMethod.Get, $"greenhouses/{greenhouseId}/history");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _client.SendAsync(request);
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
            if (jsonObject["Temperature"] != null)
            {
                Assert.True(jsonObject["Temperature"].Type == JTokenType.Integer || jsonObject["Temperature"].Type == JTokenType.Float);
            }
            if (jsonObject["LightIntensity"] != null)
            {
                Assert.True(jsonObject["LightIntensity"].Type == JTokenType.Integer || jsonObject["LightIntensity"].Type == JTokenType.Float);
            }
            if (jsonObject["Co2Levels"] != null)
            {
                Assert.True(jsonObject["Co2Levels"].Type == JTokenType.Integer || jsonObject["Co2Levels"].Type == JTokenType.Float);
            }
            if (jsonObject["Humidity"] != null)
            {
                Assert.True(jsonObject["Humidity"].Type == JTokenType.Integer || jsonObject["Humidity"].Type == JTokenType.Float);
            }
            if (jsonObject["isWindowOpen"] != null)
            {
                Assert.True(jsonObject["isWindowOpen"].Type == JTokenType.Boolean);
            }
            if (jsonObject["date"] != null)
            {
                Assert.True(jsonObject["date"].Type == JTokenType.Date || jsonObject["date"].Type == JTokenType.String);
            }
        }
    }*/


    public void Dispose()
    {
        _cts.Cancel();
        _mockTcpClientTask.Wait();
    }
}
