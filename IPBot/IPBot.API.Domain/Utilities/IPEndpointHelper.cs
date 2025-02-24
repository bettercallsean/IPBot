using System.Globalization;
using System.Net;

namespace IPBot.API.Domain.Utilities;

public static class IPEndpointHelper
{
    public static IPEndPoint GetIPEndPoint(string address)
    {
        var endpoint = address.Split(':');
        if (endpoint.Length != 2) throw new FormatException("Invalid endpoint format");

        var ip = Dns.GetHostAddresses(endpoint[0])[0] ?? throw new FormatException("Invalid ip-adress");
        if (!int.TryParse(endpoint[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out int port))
        {
            throw new FormatException("Invalid port");
        }

        return new(ip, port);
    }
}
