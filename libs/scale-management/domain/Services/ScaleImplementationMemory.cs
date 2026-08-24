namespace MicraPro.ScaleManagement.Domain.Services;

public class ScaleImplementationMemoryService
{
    private record Entry(string Identifier, string Implementation);

    private Entry[] _memory = [];
    private readonly object _memoryLock = new();

    public void SetImplementation(string identifier, string implementation)
    {
        lock (_memoryLock)
        {
            _memory = _memory
                .Where(d => d.Identifier != identifier)
                .Append(new Entry(identifier, implementation))
                .ToArray();
        }
    }

    public string GetImplementation(string identifier) =>
        _memory.FirstOrDefault(d => d.Identifier == identifier)?.Implementation
        ?? throw new KeyNotFoundException("Failed to find Implementation!");
}
