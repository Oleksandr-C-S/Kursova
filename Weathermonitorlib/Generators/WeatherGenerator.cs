namespace WeatherMonitor.Generators;
public static class WeatherGenerator
{
        public static IEnumerable<double> TemperatureStream()
    {
        var rng = new Random();
        while (true) 
        {
            yield return Math.Round(rng.NextDouble() * 75 - 30, 1);
        }
    }
    public static IEnumerable<string> WindDirectionCycle()
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = 0;
        while (true)
        {
            yield return directions[index];
            index = (index + 1) % directions.Length; 
        }
    }
}