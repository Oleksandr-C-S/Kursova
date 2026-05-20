using System.Runtime.CompilerServices;

namespace WeatherMonitor.Streaming;
public static class WeatherStream
{
    public static async IAsyncEnumerable<double> ReadTemperaturesFromCsv(
        string path,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var reader = new StreamReader(path);
        string? line;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;
            if (double.TryParse(
                    line.Trim(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double temp))
            {
                yield return temp;
            }
        }
    }
}