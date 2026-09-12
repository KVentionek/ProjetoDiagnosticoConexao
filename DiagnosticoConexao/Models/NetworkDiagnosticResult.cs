using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagnosticoConexao.Models
{
    public class NetworkDiagnosticResult
    {
        public NetworkInfo? Network { get; set; }

        public bool GatewayAvailable { get; set; }

        public long GatewayLatency { get; set; }

        public int GatewayTotalPackets { get; set; }

        public int GatewaySuccessfulPackets { get; set; }

        public int GatewayLostPackets { get; set; }

        public double GatewayPacketLossPercentage { get; set; }

        public long GatewayMinimumLatency { get; set; }

        public long GatewayMaximumLatency { get; set; }

        public double GatewayAverageLatency { get; set; }

        public bool DnsAvailable { get; set; }

        public string? DnsResult { get; set; }

        public bool InternetAvailable { get; set; }

        public long InternetLatency { get; set; }

        public int InternetTotalPackets { get; set; }

        public int InternetSuccessfulPackets { get; set; }

        public int InternetLostPackets { get; set; }

        public double InternetPacketLossPercentage { get; set; }

        public long InternetMinimumLatency { get; set; }

        public long InternetMaximumLatency { get; set; }

        public double InternetAverageLatency { get; set; }

        public List<TracerouteHop> TracerouteHops { get; set; } = new();

        public List<PortTestResult> PortTests { get; set; } = new();

        public List<SecurityFinding> SecurityAnalysis { get; set; } = new();
        
        public SecuritySummary? SecuritySummary { get; set; }

        public List<PortTestResult> LocalPorts { get; set; } = new();
    }
}