using System.Diagnostics;

namespace WeatherMonitor.Generators;

public static class TimeoutIterator{
    public static void ConsumeWithTimeout(
        IEnumerable<double> source,
        double timeoutSeconds,
        out double avarage,
        out double total)
    {
        var sw = Stopwatch.StarNew();
        var values = new List<double>();
        int count = 0;

        using var enumerator = source.GetEnumerator();
         while (enumerator.MoveNext() &&
               sw.Elapsed.TotalSeconds < timeoutSeconds)
        {
            double val = enumerator.Current;
            values.Add(val);
            count++;

              Console.Write($"\r[{count,5}] Temp: {val,7:F1}°C | " +
                          $"Avg: {(values.Count > 0 ? values.Average() : 0),6:F2}°C  ");
        }
         Console.WriteLine(); 

        average = values.Count > 0 ? values.Average() : 0; 
        total   = values.Count > 0 ? values.Sum()     : 0; 
    }
    public static WeatherStats ConsumeWithStats(
        IEnumerable<double> source,
        double timeoutSecons)
    {
        var sw = Stopwatch.StartNew();
        var values = new List<double>();

        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext()&& sw.Elapsed.TotalSeconds < timeoutSeconds)
        values.Add(enumerator.Current);

        if (values.Count == 0)
        return new WeatherStats(0,0,0,0,0);

        return new WeatherStats(
            Count: values.Count,
            Avarage: values.Avarage(),
            Total: values.Sum(),
            Min: values.Min(),
            Max: values.Max()
        );
    }
}
public record WeatherStats(int Count,double Avarage,double Total,double Min, double Max)
{
    public override string ToString() =>
    "Кількість: {Count} |  Середнє: {Avarage:F2}°C | " +
    "Мін: {Min:F1}°C | Макс: {Max:F1}°C | Сума: {Total:F1}";
}