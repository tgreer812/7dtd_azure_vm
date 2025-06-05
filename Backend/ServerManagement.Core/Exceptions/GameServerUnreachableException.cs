namespace ServerManagement.Core.Exceptions;

public class GameServerUnreachableException : Exception
{
    public GameServerUnreachableException() { }
    
    public GameServerUnreachableException(string message) : base(message) { }
    
    public GameServerUnreachableException(string message, Exception innerException) : base(message, innerException) { }
}