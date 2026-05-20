namespace WeatherMonitor.DataStructures;
public record QueueItem<T>(
    T Value,
    int Priority,
    long InsertionOrder
);
public class BiDiPriorityQueue<T>
{
    private readonly LinkedList<QueueItem<T>> _list = new();
    private long _insertionCounter = 0;
    public int Count => _list.Count;
    public bool IsEmpty => _list.Count == 0;
    public void Enqueue(T item, int priority)
    {
        var node = new QueueItem<T>(item, priority, _insertionCounter++);
        _list.AddLast(node);
    }
    public override string ToString()
    {
        return "[" + string.Join(", ", _list.Select(x => $"{x.Value}(p={x.Priority})")) + "]";
    }
}