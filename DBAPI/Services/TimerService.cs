using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Repositories;

public class TimerService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _services;
    private readonly ILogger<TimerService> _logger;

    public TimerService(IServiceProvider services, ILogger<TimerService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DuplicateEarliestGreenHouseData, null, TimeSpan.Zero, TimeSpan.TimeSpan.FromDays(1));
        return Task.CompletedTask;
    }
    private async void DuplicateEarliestGreenHouseData(object state)
    {
        using var scope = _services.CreateScope();
        var greenHouseRepository = scope.ServiceProvider.GetRequiredService<GreenHouseDateListRepository>();

        foreach (var id in new[] { 1, 10 })
        {
            var greenHouseDateList = await greenHouseRepository.GetByIdAsync(id);
            if (greenHouseDateList != null)
            {
                var earliestGreenHouse = greenHouseDateList.GreenHouses.OrderBy(gh => gh.Date).LastOrDefault();
                if (earliestGreenHouse != null)
                {
                    var newGreenHouse = new GreenHouse(
                        earliestGreenHouse.GreenHouseName,
                        earliestGreenHouse.Description,
                        earliestGreenHouse.Temperature,
                        earliestGreenHouse.LightIntensity,
                        earliestGreenHouse.Co2Levels,
                        earliestGreenHouse.Humidity,
                        earliestGreenHouse.IsWindowOpen,
                        DateTime.UtcNow
                    )
                    {
                        GreenHouseId = earliestGreenHouse.GreenHouseId
                    };

                    greenHouseDateList.GreenHouses.Add(newGreenHouse);
                    await greenHouseRepository.UpdateAsync(greenHouseDateList);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
