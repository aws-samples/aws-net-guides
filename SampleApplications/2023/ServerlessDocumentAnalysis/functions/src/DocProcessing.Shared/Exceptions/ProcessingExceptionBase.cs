namespace DocProcessing.Shared.Exceptions;

public abstract class ProcessingExceptionBase(string id, string message) : Exception(message)
{
    public string Id { get; set; } = id;
}
