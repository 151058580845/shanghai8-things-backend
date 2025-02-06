using System.Net.NetworkInformation;

namespace Hgzn.Mes.Infrastructure.Utilities;

public static class IpExtension
{
    public static string GetMacAddress()
    {
        foreach (var network in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (network.OperationalStatus == OperationalStatus.Up)
            {
                return string.Join(":", network.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
            }
        }
        return "+";
    }
}