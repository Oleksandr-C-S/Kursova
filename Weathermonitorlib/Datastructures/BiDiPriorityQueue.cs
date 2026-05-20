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
    public T Peek(string mode)
    {
        EnsureNotEmpty();
        return FindItem(mode).Value; 
    }
    public T Dequeue(string mode)
    {
        EnsureNotEmpty();
        var item = FindItem(mode); 
        _list.Remove(item);     
        return item.Value;
    }
    private QueueItem<T> FindItem(string mode) => mode switch
    {

        "highest" => _list.OrderByDescending(x => x.Priority).First(),
        "lowest"  => _list.OrderBy(x => x.Priority).First(),
        "oldest"  => _list.OrderBy(x => x.InsertionOrder).First(),
        "newest"  => _list.OrderByDescending(x => x.InsertionOrder).First(),
        _ => throw new ArgumentException($"Невідомий режим: '{mode}'. " +
                                         "Допустимі: highest, lowest, oldest, newest")
    };
    private void EnsureNotEmpty()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Черга порожня.");
    }
    public override string ToString()
    {
        return "[" + string.Join(", ", _list.Select(x => $"{x.Value}(p={x.Priority})")) + "]";
    }
}