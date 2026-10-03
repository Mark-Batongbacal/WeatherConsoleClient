using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;

namespace WeatherConsoleClient.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherApiClient _weatherApiClient;

    public WeatherService(
        IWeatherApiClient weatherApiClient)
    {
        _weatherApiClient = weatherApiClient;
    }

    public async Task<CurrentWeatherDto?> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken)
    {
        ValidateCity(city);

        return await _weatherApiClient
            .GetCurrentWeatherAsync(
                city,
                cancellationToken);
    }

    public async Task<ForecastDto?> GetForecastAsync(
        string city,
        CancellationToken cancellationToken)
    {
        ValidateCity(city);

        return await _weatherApiClient
            .GetForecastAsync(
                city,
                cancellationToken);
    }

    public ForecastSummaryDto? CreateForecastSummary(
        IEnumerable<ForecastItemDto> forecastItems)
    {
        var items = forecastItems.ToList();

        if (items.Count == 0)
        {
            return null;
        }

        return new ForecastSummaryDto
        {
            HighestTemperature = items.Max(item => item.Main.Temperature),
            LowestTemperature = items.Min(item => item.Main.Temperature),
            AverageTemperature = items.Average(item => item.Main.Temperature),
            HighestRainChance = items.Max(item => item.ProbabilityOfPrecipitation) * 100m
        };
    }

    private static void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "City is required.");
        }
    }
}
