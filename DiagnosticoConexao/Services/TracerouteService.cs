using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class TracerouteService
    {
        public async Task<List<TracerouteHop>> TraceAsync(
            string destination,
            int maxHops = 30)
        {
            List<TracerouteHop> hops = new List<TracerouteHop>();

            IPAddress[] addresses =
                await Dns.GetHostAddressesAsync(destination);

            IPAddress? destinationAddress =
                addresses.FirstOrDefault(
                    address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                );

            if (destinationAddress == null)
            {
                throw new Exception(
                    "Não foi possível encontrar um endereço IPv4 para o destino."
                );
            }

            using Ping ping = new Ping();

            for (int ttl = 1; ttl <= maxHops; ttl++)
            {
                PingOptions options =
                    new PingOptions(ttl, true);

                byte[] buffer = new byte[32];

                PingReply reply =
                    await ping.SendPingAsync(
                        destinationAddress,
                        3000,
                        buffer,
                        options
                    );

                if (reply.Status == IPStatus.Success ||
                    reply.Status == IPStatus.TtlExpired)
                {
                    hops.Add(
                        new TracerouteHop
                        {
                            HopNumber = ttl,
                            Address = reply.Address?.ToString(),
                            Latency = reply.RoundtripTime,
                            Success = true
                        }
                    );
                }
                else
                {
                    hops.Add(
                        new TracerouteHop
                        {
                            HopNumber = ttl,
                            Address = "*",
                            Latency = 0,
                            Success = false
                        }
                    );
                }

                if (reply.Status == IPStatus.Success)
                {
                    break;
                }
            }

            return hops;
        }        
    }
}