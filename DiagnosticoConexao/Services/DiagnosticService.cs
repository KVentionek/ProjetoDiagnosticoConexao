using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class DiagnosticService
    {
        private readonly NetworkService _networkService;
        private readonly PingService _pingService;
        private readonly DnsService _dnsService;
        private readonly TracerouteService _tracerouteService;
        private readonly PortService _portService;
        private readonly SecurityAnalysisService _securityAnalysisService;
        private readonly LocalPortService _localPortService;

        public DiagnosticService()
        {
            _networkService = new NetworkService();
            _pingService = new PingService();
            _dnsService = new DnsService();
            _tracerouteService = new TracerouteService();
            _portService = new PortService();
            _securityAnalysisService = new SecurityAnalysisService();
            _localPortService = new LocalPortService();
        }

        public async Task<NetworkDiagnosticResult?> DiagnoseAsync()
        {
            NetworkInfo? networkInfo =
                _networkService.GetNetworkInfo();

            if (networkInfo == null)
            {
                return null;
            }

            NetworkDiagnosticResult result =
                new NetworkDiagnosticResult
                {
                    Network = networkInfo
                };

            PingResult gatewayResult =
                await _pingService.TestAsync(networkInfo.Gateway!, 10);

            result.GatewayAvailable = gatewayResult.Success;
            result.GatewayLatency = gatewayResult.Latency;

            result.GatewayTotalPackets =
                gatewayResult.TotalPackets;

            result.GatewaySuccessfulPackets =
                gatewayResult.SuccessfulPackets;

            result.GatewayLostPackets =
                gatewayResult.LostPackets;

            result.GatewayPacketLossPercentage =
                gatewayResult.PacketLossPercentage;

            result.GatewayMinimumLatency =
                gatewayResult.MinimumLatency;

            result.GatewayMaximumLatency =
                gatewayResult.MaximumLatency;

            result.GatewayAverageLatency =
                gatewayResult.AverageLatency;

            DnsResult dnsResult =
                await _dnsService.ResolveAsync("google.com");

            result.DnsAvailable = dnsResult.Success;
            result.DnsResult = dnsResult.Address;

            PingResult internetResult =
                await _pingService.TestAsync("8.8.8.8", 10);

            result.InternetAvailable = internetResult.Success;
            result.InternetLatency = internetResult.Latency;

            result.InternetTotalPackets =
                internetResult.TotalPackets;

            result.InternetSuccessfulPackets =
                internetResult.SuccessfulPackets;

            result.InternetLostPackets =
                internetResult.LostPackets;

            result.InternetPacketLossPercentage =
                internetResult.PacketLossPercentage;

            result.InternetMinimumLatency =
                internetResult.MinimumLatency;

            result.InternetMaximumLatency =
                internetResult.MaximumLatency;

            result.InternetAverageLatency =
                internetResult.AverageLatency;

            List<TracerouteHop> tracerouteHops =
                await _tracerouteService.TraceAsync("google.com");

            result.TracerouteHops = tracerouteHops;

            int[] portsToTest =
            {
                22,
                80,
                443,
                3389
            };

            foreach (int port in portsToTest)
            {
                PortTestResult portResult =
                    await _portService.TestAsync("google.com", port);

                result.PortTests.Add(portResult);
            }

            List<PortTestResult> localPorts =
                _localPortService.GetListeningPorts();

            result.LocalPorts = localPorts;

            List<SecurityFinding> securityAnalysis =
                _securityAnalysisService.AnalyzePorts(
                    result.PortTests
                );

            List<SecurityFinding> localSecurityAnalysis =
                _securityAnalysisService.AnalyzeLocalPorts(
                    result.LocalPorts
                );

            securityAnalysis.AddRange(localSecurityAnalysis);

            result.SecurityAnalysis = securityAnalysis;

            result.SecuritySummary =
                _securityAnalysisService.GenerateSummary(
                    result.SecurityAnalysis
                );

            return result;
        }
    }
}