using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Repositories;
using Microsoft.Extensions.Logging;

namespace WebAPI.Services
{
    public class WindowStatusService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WindowStatusService> _logger;

        public WindowStatusService(IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider, ILogger<WindowStatusService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var greenHouseRepository = scope.ServiceProvider.GetRequiredService<GreenHouseRepository>();

                        var httpClient = _httpClientFactory.CreateClient("IOTController");
                        var response = await httpClient.GetAsync("IOT/1/getStatus", stoppingToken);

                        if (response.IsSuccessStatusCode)
                        {
                            var status = await response.Content.ReadAsStringAsync();
                            bool isWindowOpen = status.Equals("Open", StringComparison.OrdinalIgnoreCase);

                            await greenHouseRepository.UpdateFieldAsync("1", "IsWindowOpen", isWindowOpen);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _logger.LogWarning("IOTController returned 404 for IOT/1/getStatus: No client connected.");
                        }
                        else
                        {
                            _logger.LogError($"Unexpected status code {response.StatusCode} returned from IOTController.");
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("IOTController returned 404 for IOT/1/getStatus: No client connected.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating window status");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
