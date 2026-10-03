using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class ForecastCloudsDto
{
    [JsonPropertyName("all")]
    public int Percentage { get; set; }
}