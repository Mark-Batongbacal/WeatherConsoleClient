using WeatherConsoleClient.Application.DTOs;

namespace WeatherConsoleClient.Presentation;

public interface IWeatherFormatter
{
    string FormatCurrentWeather(CurrentWeatherDto weather);

    string FormatForecast(ForecastDto forecast);

    string FormatCurrentWeather(
        CurrentWeatherDto weather,
        TemperatureUnit temperatureUnit);

    string FormatForecast(
        ForecastDto forecast,
        IEnumerable<ForecastItemDto> forecastItems,
        TemperatureUnit temperatureUnit);

    string FormatForecastSummary(
        ForecastSummaryDto? summary,
        TemperatureUnit temperatureUnit);
}
