using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagnosticoConexao.Models
{
    public class PingResult
    {
        public bool Success { get; set; }

        public long Latency { get; set; }

        public string? Status { get; set; }

        public int TotalPackets { get; set; }

        public int SuccessfulPackets { get; set; }

        public int LostPackets { get; set; }

        public double PacketLossPercentage { get; set; }

        public long MinimumLatency { get; set; }

        public long MaximumLatency { get; set; }

        public double AverageLatency { get; set; }
    }
}