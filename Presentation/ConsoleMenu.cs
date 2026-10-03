using System.Globalization;
using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;

namespace WeatherConsoleClient.Presentation;

public class ConsoleMenu
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherFormatter _weatherFormatter;
    private TemperatureUnit _temperatureUnit = TemperatureUnit.Celsius;

    public ConsoleMenu(
        IWeatherService weatherService,
        IWeatherFormatter weatherFormatter)
    {
        _weatherService = weatherService;
        _weatherFormatter = weatherFormatter;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            DisplayMenu();

            switch (Console.ReadLine())
            {
                case "1":
                    await ShowCurrentWeatherAsync(cancellationToken);
                    break;
                case "2":
                    await ShowForecastAsync(cancellationToken);
                    break;
                case "3":
                    await ShowDashboardAsync(cancellationToken);
                    break;
                case "4":
                    SelectTemperatureUnit();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }

    private void DisplayMenu()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }

        Console.WriteLine("========================================");
        Console.WriteLine("       WEATHER CONSOLE CLIENT");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine("1. Current Weather");
        Console.WriteLine("2. 5-Day / 3-Hour Forecast");
        Console.WriteLine("3. Weather Dashboard");
        Console.WriteLine($"4. Temperature Unit ({_temperatureUnit})");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    private void SelectTemperatureUnit()
    {
        Console.WriteLine();
        Console.WriteLine("Select temperature unit:");
        Console.WriteLine("1. Celsius");
        Console.WriteLine("2. Fahrenheit");
        Console.Write("Enter your choice: ");

        _temperatureUnit = Console.ReadLine() switch
        {
            "1" => TemperatureUnit.Celsius,
            "2" => TemperatureUnit.Fahrenheit,
            _ => _temperatureUnit
        };
    }

    private async Task ShowCurrentWeatherAsync(CancellationToken cancellationToken)
    {
        var city = ReadCity();
        if (city is null)
        {
            return;
        }

        var weather = await _weatherService.GetCurrentWeatherAsync(city, cancellationToken);
        if (weather is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatCurrentWeather(weather, _temperatureUnit));
    }

    private async Task ShowForecastAsync(CancellationToken cancellationToken)
    {
        var city = ReadCity();
        if (city is null)
        {
            return;
        }

        var forecast = await _weatherService.GetForecastAsync(city, cancellationToken);
        if (forecast is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        var filteredItems = FilterForecastItems(forecast.Items).ToList();
        var summary = _weatherService.CreateForecastSummary(filteredItems);

        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatForecast(forecast, filteredItems, _temperatureUnit));
        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatForecastSummary(summary, _temperatureUnit));
    }

    private async Task ShowDashboardAsync(CancellationToken cancellationToken)
    {
        var city = ReadCity();
        if (city is null)
        {
            return;
        }

        var currentWeatherTask = _weatherService.GetCurrentWeatherAsync(city, cancellationToken);
        var forecastTask = _weatherService.GetForecastAsync(city, cancellationToken);
        await Task.WhenAll(currentWeatherTask, forecastTask);

        var currentWeather = await currentWeatherTask;
        var forecast = await forecastTask;
        if (currentWeather is null || forecast is null)
        {
            Console.WriteLine("Unable to retrieve weather information.");
            return;
        }

        var summary = _weatherService.CreateForecastSummary(forecast.Items);

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("WEATHER DASHBOARD");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatCurrentWeather(currentWeather, _temperatureUnit));
        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatForecast(forecast, forecast.Items, _temperatureUnit));
        Console.WriteLine();
        Console.WriteLine(_weatherFormatter.FormatForecastSummary(summary, _temperatureUnit));
    }

    private static string? ReadCity()
    {
        Console.WriteLine();
        Console.Write("Enter city: ");

        var city = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("City is required.");
            return null;
        }

        return city;
    }

    private static IEnumerable<ForecastItemDto> FilterForecastItems(
        IEnumerable<ForecastItemDto> forecastItems)
    {
        Console.WriteLine();
        Console.WriteLine("Filter forecast:");
        Console.WriteLine("1. Show all forecast entries");
        Console.WriteLine("2. Show today's forecast");
        Console.WriteLine("3. Show tomorrow's forecast");
        Console.Write("Enter your choice: ");

        var selectedDate = Console.ReadLine() switch
        {
            "2" => DateTime.Today,
            "3" => DateTime.Today.AddDays(1),
            _ => (DateTime?)null
        };

        return selectedDate is null
            ? forecastItems
            : forecastItems.Where(item =>
                DateTime.TryParseExact(
                    item.DateTimeText,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var forecastDate) &&
                forecastDate.Date == selectedDate.Value.Date);
    }
}
