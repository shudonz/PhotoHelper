using System.Net.Http;
using System.Text.Json;

namespace PhotoHelper;

public class GeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PhotoHelper/1.0");
    }

    public async Task<(double Latitude, double Longitude)?> GeocodeAddressAsync(string address)
    {
        try
        {
            // Using Nominatim OpenStreetMap API (free, no API key required)
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(content);

            if (results != null && results.Count > 0)
            {
                var result = results[0];
                if (double.TryParse(result.lat, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(result.lon, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out var lon))
                {
                    return (lat, lon);
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Geocoding failed: {ex.Message}", ex);
        }
    }

    private class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
    }
}
