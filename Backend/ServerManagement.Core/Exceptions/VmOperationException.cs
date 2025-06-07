namespace ServerManagement.Core.Exceptions;

public class VmOperationException : Exception
{
    public VmOperationException() { }
    
    public VmOperationException(string message) : base(message) { }
    
    public VmOperationException(string message, Exception innerException) : base(message, innerException) { }
}