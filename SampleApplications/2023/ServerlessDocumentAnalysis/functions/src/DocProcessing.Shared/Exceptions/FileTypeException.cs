namespace DocProcessing.Shared.Exceptions;

public class FileTypeException(string id, string message) : ProcessingExceptionBase(id, message)
{
}

