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
}