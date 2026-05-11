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
}