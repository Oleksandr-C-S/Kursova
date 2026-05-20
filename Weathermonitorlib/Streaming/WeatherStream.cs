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
    public static async Task<(double Min, double Max, double Avg, long Count)>
        ComputeStatsAsync(IAsyncEnumerable<double> stream)
    {
        double min = double.MaxValue;
        double max = double.MinValue;
        double sum = 0;
        long count = 0;
        await foreach (var temp in stream)
        {
            if (temp < min) min = temp;
            if (temp > max) max = temp;
            sum += temp;
            count++;
            if (count % 10_000 == 0)
                Console.Write($"\r  Оброблено: {count:N0} записів...");
        }
        Console.WriteLine();
        return (
            min,
            max,
            count > 0 ? sum / count : 0,
            count
        );
    }
}