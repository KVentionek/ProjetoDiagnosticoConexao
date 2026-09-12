using System.Net.NetworkInformation;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class PingService
    {
        public async Task<PingResult> TestAsync(
            string address,
            int numberOfTests = 5)
        {
            int successfulPackets = 0;
            int lostPackets = 0;

            List<long> latencies = new List<long>();

            for (int i = 0; i < numberOfTests; i++)
            {
                using Ping ping = new Ping();

                PingReply reply =
                    await ping.SendPingAsync(address, 3000);

                if (reply.Status == IPStatus.Success)
                {
                    successfulPackets++;
                    latencies.Add(reply.RoundtripTime);
                }
                else
                {
                    lostPackets++;
                }
            }

            bool success = successfulPackets > 0;

            long minimumLatency = 0;
            long maximumLatency = 0;
            double averageLatency = 0;

            if (latencies.Count > 0)
            {
                minimumLatency = latencies.Min();
                maximumLatency = latencies.Max();
                averageLatency = latencies.Average();
            }

            double packetLossPercentage =
                (double)lostPackets / numberOfTests * 100;

            return new PingResult
            {
                Success = success,

                Latency = latencies.Count > 0
                    ? latencies.Last()
                    : 0,

                Status = success
                    ? "Success"
                    : "Failed",

                TotalPackets = numberOfTests,

                SuccessfulPackets = successfulPackets,

                LostPackets = lostPackets,

                PacketLossPercentage = packetLossPercentage,

                MinimumLatency = minimumLatency,

                MaximumLatency = maximumLatency,

                AverageLatency = averageLatency
            };
        }
    }
}