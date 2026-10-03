namespace WeatherConsoleClient.Application.DTOs;

public class ForecastSummaryDto
{
    public decimal HighestTemperature { get; set; }

    public decimal LowestTemperature { get; set; }

    public decimal AverageTemperature { get; set; }

    public decimal HighestRainChance { get; set; }
}
