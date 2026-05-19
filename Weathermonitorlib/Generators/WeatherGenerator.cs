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
 public static IEnumerable<long> FibonacciSequence()
    {
        long a = 0, b = 1;
        while (true)
        {
            yield return a;
            (a, b) = (b, a + b); 
        }
    }
    public static IEnumerable<double> WindSpeedStream()
    {
        var rng = new Random();

        while (true)
        {
            double speed = Math.Abs(rng.NextGaussian() * 15);
            yield return Math.Round(Math.Min(speed, 120), 1);
        }
    }
}
public static class RandomExtensions
{
    public static double NextGaussian(this Random rng)
    {
        double u1 = 1.0 - rng.NextDouble(); 
        double u2 = 1.0 - rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
    }
}