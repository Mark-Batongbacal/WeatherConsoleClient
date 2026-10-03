using System.Globalization;
using System.Text;
using WeatherConsoleClient.Application.DTOs;

namespace WeatherConsoleClient.Presentation;

public class ConsoleWeatherFormatter : IWeatherFormatter
{
    private const decimal RainAlertThreshold = 60m;
    private const decimal HotWeatherThresholdCelsius = 35m;

    public string FormatCurrentWeather(CurrentWeatherDto weather)
    {
        return FormatCurrentWeather(weather, TemperatureUnit.Celsius);
    }

    public string FormatForecast(ForecastDto forecast)
    {
        return FormatForecast(
            forecast,
            forecast.Items,
            TemperatureUnit.Celsius);
    }

    public string FormatCurrentWeather(
        CurrentWeatherDto weather,
        TemperatureUnit temperatureUnit)
    {
        var output = new StringBuilder();

        output.AppendLine("========================================");
        output.AppendLine("CURRENT WEATHER");
        output.AppendLine("========================================");
        output.AppendLine();
        output.AppendLine($"City        : {weather.Name}");
        output.AppendLine($"Temperature : {FormatTemperature(weather.Main.Temperature, temperatureUnit)}");
        output.AppendLine($"Feels Like  : {FormatTemperature(weather.Main.FeelsLike, temperatureUnit)}");
        output.AppendLine($"Humidity    : {weather.Main.Humidity} %");
        output.AppendLine($"Pressure    : {weather.Main.Pressure} hPa");

        if (weather.Weather.Count > 0)
        {
            output.AppendLine($"Condition   : {weather.Weather[0].Description}");
        }

        output.AppendLine($"Wind Speed  : {weather.Wind.Speed:F2} m/s");

        if (weather.Main.Temperature > HotWeatherThresholdCelsius)
        {
            output.AppendLine();
            output.AppendLine("WEATHER ALERT:");
            output.AppendLine("High temperature detected.");
        }

        return output.ToString().TrimEnd();
    }

    public string FormatForecast(
        ForecastDto forecast,
        IEnumerable<ForecastItemDto> forecastItems,
        TemperatureUnit temperatureUnit)
    {
        var output = new StringBuilder();
        var items = forecastItems.ToList();

        output.AppendLine("========================================");
        output.AppendLine("5-DAY / 3-HOUR FORECAST");
        output.AppendLine("========================================");
        output.AppendLine();
        output.AppendLine($"City: {forecast.City.Name}");
        output.AppendLine();

        if (items.Count == 0)
        {
            output.Append("No forecast entries match this filter.");
            return output.ToString();
        }

        foreach (var item in items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";
            var precipitation = item.ProbabilityOfPrecipitation * 100m;

            output.AppendLine(
                $"{item.DateTimeText,-20}" +
                $"{FormatTemperature(item.Main.Temperature, temperatureUnit),9}   " +
                $"{condition,-18}" +
                $"{precipitation,5:F0}%");

            if (precipitation >= RainAlertThreshold)
            {
                output.AppendLine("RAIN ALERT: High probability of precipitation.");
            }

            if (item.Main.Temperature > HotWeatherThresholdCelsius)
            {
                output.AppendLine("WEATHER ALERT: High temperature detected.");
            }
        }

        return output.ToString().TrimEnd();
    }

    public string FormatForecastSummary(
        ForecastSummaryDto? summary,
        TemperatureUnit temperatureUnit)
    {
        if (summary is null)
        {
            return "FORECAST SUMMARY\n----------------------------------------\nNo forecast entries match this filter.";
        }

        return string.Join(
            Environment.NewLine,
            "FORECAST SUMMARY",
            "----------------------------------------",
            $"Highest Temperature : {FormatTemperature(summary.HighestTemperature, temperatureUnit)}",
            $"Lowest Temperature  : {FormatTemperature(summary.LowestTemperature, temperatureUnit)}",
            $"Average Temperature : {FormatTemperature(summary.AverageTemperature, temperatureUnit)}",
            $"Highest Rain Chance : {summary.HighestRainChance:F0} %");
    }

    private static string FormatTemperature(
        decimal temperatureCelsius,
        TemperatureUnit temperatureUnit)
    {
        var temperature = temperatureUnit == TemperatureUnit.Fahrenheit
            ? temperatureCelsius * 9m / 5m + 32m
            : temperatureCelsius;
        var unit = temperatureUnit == TemperatureUnit.Fahrenheit
            ? "F"
            : "C";

        return string.Format(
            CultureInfo.InvariantCulture,
            "{0:F1} {1}",
            temperature,
            unit);
    }
}
