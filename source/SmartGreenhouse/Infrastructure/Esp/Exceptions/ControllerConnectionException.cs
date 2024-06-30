using System.Net;

namespace Infrastructure.Esp.Exceptions;

public class ControllerConnectionException(IPEndPoint iPEnd, string? message, Exception? innerException)
    : Exception(message, innerException)
{
    public IPEndPoint IpEndPoint { get; init; } = iPEnd;
}