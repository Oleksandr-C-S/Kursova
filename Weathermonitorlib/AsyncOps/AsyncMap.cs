namespace WeatherMonitor.AsyncOps;

public static class AsyncMap
{
    public static void MapCallback<TIn, TOut>(
        IEnumerable<TIn> source,
        Func<TIn, TOut> selector,
        Action<TOut[]> callback,
        Action<Exception>? onError = null)
    {
        Task.Run(() =>
        {
            try
            {
                var result = new List<TOut>();
                foreach (var item in source)
                    result.Add(selector(item));

                callback(result.ToArray()); 
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        });
    }
    public static Task<TOut[]> MapAsync<TIn, TOut>(
        IEnumerable<TIn> source,
        Func<TIn, Task<TOut>> asyncSelector,
        CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            var results = new List<TOut>();

            foreach (var item in source)
            {
                ct.ThrowIfCancellationRequested();
                results.Add(await asyncSelector(item));
            }

            return results.ToArray();
        }, ct); 
    }
    public static async Task DemoAsync()
    {
        Console.WriteLine(" Async Map Demo ");
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(5));
        double[] celsius = { -40.0, -20.0, 0.0, 20.0, 37.0, 100.0 };
        try
        {
            var fahrenheit = await MapAsync(
                celsius,
                async temp =>
                {
                    await Task.Delay(100, cts.Token);
                    return temp * 9.0 / 5 + 32; 
                },
                cts.Token);
            Console.WriteLine("Celsius → Fahrenheit:");
            for (int i = 0; i < celsius.Length; i++)
                Console.WriteLine($"  {celsius[i],6:F1}°C = {fahrenheit[i],7:F1}°F");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[AsyncMap] Операцію скасовано.");
        }
    }
}