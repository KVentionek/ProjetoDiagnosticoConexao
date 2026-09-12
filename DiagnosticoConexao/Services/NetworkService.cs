using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class NetworkService
    {
        public NetworkInfo? GetNetworkInfo()
        {
            NetworkInterface[] interfaces =
                NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in interfaces)
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                    continue;

                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                    continue;

                IPInterfaceProperties properties =
                    networkInterface.GetIPProperties();

                IPAddress? ipv4 = null;
                string? subnetMask = null;

                foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipv4 = ip.Address;
                        subnetMask = ip.IPv4Mask?.ToString();
                        break;
                    }
                }

                if (ipv4 == null || subnetMask == null)
                    continue;

                IPAddress? gateway = null;

                foreach (GatewayIPAddressInformation gatewayInfo in properties.GatewayAddresses)
                {
                    if (gatewayInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        gateway = gatewayInfo.Address;
                        break;
                    }
                }

                if (gateway == null)
                    continue;

                List<string> dnsServers = new List<string>();

                foreach (IPAddress dnsAddress in properties.DnsAddresses)
                {
                    if (dnsAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        dnsServers.Add(dnsAddress.ToString());
                    }
                }

                return new NetworkInfo
                {
                    Name = networkInterface.Name,
                    Type = networkInterface.NetworkInterfaceType.ToString(),
                    IPv4 = ipv4.ToString(),
                    SubnetMask = subnetMask,
                    Cidr = CalculateCidr(IPAddress.Parse(subnetMask!)),
                    NetworkAddress = CalculateNetworkAddress(
                            ipv4,
                            IPAddress.Parse(subnetMask)
                        ),
                    DnsServers = dnsServers,
                    Gateway = gateway.ToString()
                };
            }

            return null;
        }

        private int CalculateCidr(IPAddress subnetMask)
        {
            byte[] bytes = subnetMask.GetAddressBytes();

            int cidr = 0;

            foreach (byte b in bytes)
            {
                cidr += Convert.ToString(b, 2)
                    .Count(bit => bit == '1');
            }

            return cidr;
        }

        private string CalculateNetworkAddress(
            IPAddress ipv4,
            IPAddress subnetMask)
        {
            byte[] ipBytes = ipv4.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            byte[] networkBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] =
                    (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes).ToString();
        }
    }
}