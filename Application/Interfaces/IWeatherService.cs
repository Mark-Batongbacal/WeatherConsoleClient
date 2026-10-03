using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherConsoleClient.Application.DTOs;

namespace WeatherConsoleClient.Application.Interfaces;

public interface IWeatherService
{
    Task<CurrentWeatherDto?> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken);

    Task<ForecastDto?> GetForecastAsync(
        string city,
        CancellationToken cancellationToken);
}