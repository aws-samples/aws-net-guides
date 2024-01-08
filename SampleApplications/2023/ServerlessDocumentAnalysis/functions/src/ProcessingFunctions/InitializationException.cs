using DocProcessing.Shared.Exceptions;

namespace InitializeProcessing;

public class InitializationException(string docKey, string message) : ProcessingExceptionBase(docKey, message)
{
}
