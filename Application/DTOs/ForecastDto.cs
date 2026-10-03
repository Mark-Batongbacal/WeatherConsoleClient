using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class ForecastDto
{
    [JsonPropertyName("cnt")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public List<ForecastItemDto> Items { get; set; } = [];

    [JsonPropertyName("city")]
    public ForecastCityDto City { get; set; } = new();
}