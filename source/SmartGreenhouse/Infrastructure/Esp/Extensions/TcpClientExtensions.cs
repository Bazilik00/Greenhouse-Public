using System.Net;
using System.Net.Sockets;

namespace Infrastructure.Esp.Extensions;

public static class TcpClientExtensions
{
    public static void Connect(this TcpClient client, IPEndPoint iPEnd, int connectionTimeout)
    {
        var result = client.BeginConnect(iPEnd.Address, iPEnd.Port, null, null);

        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectionTimeout));

        if (!success)
        {
            throw new TimeoutException();
        }

        client.EndConnect(result);
    }
}