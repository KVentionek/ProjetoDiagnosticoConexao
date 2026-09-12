using DiagnosticoConexao.Models;
using DiagnosticoConexao.Services;

Console.WriteLine("=================================");
Console.WriteLine("     DIAGNÓSTICO DE CONEXÃO");
Console.WriteLine("=================================");
Console.WriteLine();

DiagnosticService diagnosticService =
    new DiagnosticService();

SecurityPresentationService securityPresentationService =
    new SecurityPresentationService();

AnalysisService analysisService =
    new AnalysisService();

NetworkDiagnosticResult? result =
    await diagnosticService.DiagnoseAsync();

if (result == null)
{
    Console.WriteLine("✗ Nenhuma conexão de rede encontrada.");
    return;
}

Console.WriteLine($"Nome: {result.Network.Name}");
Console.WriteLine($"Tipo: {result.Network.Type}");
Console.WriteLine($"IPv4: {result.Network.IPv4}");
Console.WriteLine($"Máscara: {result.Network.SubnetMask}");
Console.WriteLine($"CIDR: /{result.Network.Cidr}");
Console.WriteLine($"Rede: {result.Network.NetworkAddress}");
Console.WriteLine($"DNS: {string.Join(", ", result.Network.DnsServers)}");
Console.WriteLine($"Gateway: {result.Network.Gateway}");

Console.WriteLine();
Console.WriteLine("Testando gateway...");

if (result.GatewayAvailable)
{
    Console.WriteLine("✓ Gateway acessível");

    Console.WriteLine(
        $"  Pacotes: {result.GatewaySuccessfulPackets}/{result.GatewayTotalPackets}");

    Console.WriteLine(
        $"  Perda: {result.GatewayPacketLossPercentage:F1}%");

    Console.WriteLine(
        $"  Latência mínima: {result.GatewayMinimumLatency} ms");

    Console.WriteLine(
        $"  Latência máxima: {result.GatewayMaximumLatency} ms");

    Console.WriteLine(
        $"  Latência média: {result.GatewayAverageLatency:F1} ms");
}
else
{
    Console.WriteLine("✗ Gateway indisponível");
}

Console.WriteLine();
Console.WriteLine("Testando DNS...");

if (result.DnsAvailable)
{
    Console.WriteLine("✓ DNS funcionando");
    Console.WriteLine(
        $"  google.com → {result.DnsResult}");
}
else
{
    Console.WriteLine("✗ DNS indisponível");
}

Console.WriteLine();
Console.WriteLine("Testando Internet...");

if (result.InternetAvailable)
{
    Console.WriteLine("✓ Internet disponível");

    Console.WriteLine(
        $"  Pacotes: {result.InternetSuccessfulPackets}/{result.InternetTotalPackets}");

    Console.WriteLine(
        $"  Perda: {result.InternetPacketLossPercentage:F1}%");

    Console.WriteLine(
        $"  Latência mínima: {result.InternetMinimumLatency} ms");

    Console.WriteLine(
        $"  Latência máxima: {result.InternetMaximumLatency} ms");

    Console.WriteLine(
        $"  Latência média: {result.InternetAverageLatency:F1} ms");
}
else
{
    Console.WriteLine("✗ Internet indisponível");
}

Console.WriteLine();
Console.WriteLine("=================================");
Console.WriteLine("          TRACEROUTE");
Console.WriteLine("=================================");
Console.WriteLine();

Console.WriteLine("Destino: google.com");
Console.WriteLine();

foreach (TracerouteHop hop in result.TracerouteHops)
{
    if (hop.Success)
    {
        Console.WriteLine(
            $"{hop.HopNumber,2}   {hop.Address,-20} {hop.Latency,4} ms"
        );
    }
    else
    {
        Console.WriteLine(
            $"{hop.HopNumber,2}   *"
        );
    }
}

Console.WriteLine();
Console.WriteLine("=================================");
Console.WriteLine("         TESTE DE PORTAS");
Console.WriteLine("=================================");
Console.WriteLine();

Console.WriteLine("Host: google.com");
Console.WriteLine();

foreach (PortTestResult port in result.PortTests)
{
    string status = port.Open
        ? "ABERTA"
        : port.Status switch
        {
            "REFUSED" => "RECUSADA",
            "TIMEOUT" => "TIMEOUT",
            "HOST_NOT_FOUND" => "HOST NÃO ENCONTRADO",
            _ => "ERRO"
        };

    if (port.Open)
    {
        Console.WriteLine(
            $"Porta {port.Port,-4} → {status,-10} {port.Latency} ms"
        );
    }
    else
    {
        Console.WriteLine(
            $"Porta {port.Port,-4} → {status}"
        );
    }
}

Console.WriteLine();
Console.WriteLine("=================================");
Console.WriteLine("    AUDITORIA LOCAL DE PORTAS");
Console.WriteLine("=================================");
Console.WriteLine();

if (result.LocalPorts.Count == 0)
{
    Console.WriteLine(
        "✓ Nenhuma porta TCP está em escuta local."
    );
}
else
{
    Console.WriteLine("Portas TCP em escuta:");

    Console.WriteLine();

    foreach (PortTestResult port in result.LocalPorts)
    {
        Console.WriteLine(
            $"{port.Host,-16} Porta {port.Port,-5} → LISTENING  PID: {port.ProcessId,-6} Processo: {port.ProcessName}"
        );
    }
}

securityPresentationService.PrintSecurityAnalysis(
    result.SecurityAnalysis,
    result.SecuritySummary!
);

Console.WriteLine();
Console.WriteLine("=================================");
Console.WriteLine("             ANÁLISE");
Console.WriteLine("=================================");
Console.WriteLine();

string analysis =
    analysisService.Analyze(result);

Console.WriteLine(analysis);

Console.WriteLine();
Console.WriteLine("=================================");