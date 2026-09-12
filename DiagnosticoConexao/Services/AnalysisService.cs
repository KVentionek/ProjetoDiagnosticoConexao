using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class AnalysisService
    {
        public string Analyze(NetworkDiagnosticResult result)
        {
            List<string> analysis = new List<string>();

            // ==========================================
            // 1. GATEWAY
            // ==========================================

            if (!result.GatewayAvailable)
            {
                analysis.Add(
                    "✗ Gateway indisponível."
                );

                analysis.Add(
                    "→ Possível problema na rede local, cabo, Wi-Fi ou roteador."
                );

                return string.Join(
                    Environment.NewLine,
                    analysis
                );
            }

            if (result.GatewayPacketLossPercentage > 0)
            {
                analysis.Add(
                    $"⚠ Perda de {result.GatewayPacketLossPercentage:F1}% de pacotes no gateway."
                );

                analysis.Add(
                    "→ Possível problema na rede local ou na comunicação com o roteador."
                );
            }
            else
            {
                analysis.Add(
                    "✓ Rede local funcionando normalmente."
                );
            }

            // ==========================================
            // 2. DNS
            // ==========================================

            if (!result.DnsAvailable)
            {
                analysis.Add(
                    "✗ DNS indisponível."
                );

                analysis.Add(
                    "→ A rede local está acessível, mas a resolução de nomes falhou."
                );
            }
            else
            {
                analysis.Add(
                    "✓ DNS funcionando normalmente."
                );
            }

            // ==========================================
            // 3. INTERNET
            // ==========================================

            if (!result.InternetAvailable)
            {
                analysis.Add(
                    "✗ Internet indisponível."
                );

                if (result.GatewayAvailable)
                {
                    analysis.Add(
                        "→ O computador consegue alcançar o roteador, mas não o destino externo."
                    );
                }

                return string.Join(
                    Environment.NewLine,
                    analysis
                );
            }

            // ==========================================
            // 4. PERDA DE PACOTES NA INTERNET
            // ==========================================

            if (result.InternetPacketLossPercentage > 0)
            {
                analysis.Add(
                    $"⚠ Perda de {result.InternetPacketLossPercentage:F1}% de pacotes na Internet."
                );

                if (result.GatewayPacketLossPercentage == 0)
                {
                    analysis.Add(
                        "→ A rede local está estável. O problema pode estar além do roteador."
                    );
                }
            }
            else
            {
                analysis.Add(
                    "✓ Sem perda de pacotes na Internet."
                );
            }

            // ==========================================
            // 5. LATÊNCIA
            // ==========================================

            if (result.InternetAverageLatency < 50)
            {
                analysis.Add(
                    $"✓ Latência excelente: {result.InternetAverageLatency:F1} ms."
                );
            }
            else if (result.InternetAverageLatency < 100)
            {
                analysis.Add(
                    $"✓ Latência boa: {result.InternetAverageLatency:F1} ms."
                );
            }
            else if (result.InternetAverageLatency < 200)
            {
                analysis.Add(
                    $"⚠ Latência elevada: {result.InternetAverageLatency:F1} ms."
                );
            }
            else
            {
                analysis.Add(
                    $"✗ Latência muito alta: {result.InternetAverageLatency:F1} ms."
                );
            }

            // ==========================================
            // 6. TRACEROUTE
            // ==========================================

            bool latencyIncreaseDetected = false;

            if (result.TracerouteHops.Count > 1)
            {
                for (int i = 1; i < result.TracerouteHops.Count; i++)
                {
                    TracerouteHop previousHop =
                        result.TracerouteHops[i - 1];

                    TracerouteHop currentHop =
                        result.TracerouteHops[i];

                    if (!previousHop.Success || !currentHop.Success)
                        continue;

                    long latencyDifference =
                        currentHop.Latency - previousHop.Latency;

                    if (latencyDifference >= 50)
                    {
                        analysis.Add(
                            $"⚠ Aumento significativo de latência detectado no hop {currentHop.HopNumber}."
                        );

                        analysis.Add(
                            $"→ A latência aumentou {latencyDifference} ms em relação ao hop anterior."
                        );

                        latencyIncreaseDetected = true;

                        break;
                    }
                }
            }

            if (!latencyIncreaseDetected)
            {
                analysis.Add(
                    "✓ Nenhum aumento significativo de latência identificado no traceroute."
                );
            }

            // ==========================================
            // 7. QUALIDADE GERAL
            // ==========================================

            if (
                result.GatewayPacketLossPercentage == 0 &&
                result.InternetPacketLossPercentage == 0 &&
                result.InternetAverageLatency < 50
            )
            {
                analysis.Add(
                    "✓ Qualidade da conexão: EXCELENTE."
                );
            }
            else if (
                result.GatewayPacketLossPercentage == 0 &&
                result.InternetPacketLossPercentage <= 2 &&
                result.InternetAverageLatency < 100
            )
            {
                analysis.Add(
                    "✓ Qualidade da conexão: BOA."
                );
            }
            else if (
                result.InternetPacketLossPercentage <= 5 &&
                result.InternetAverageLatency < 200
            )
            {
                analysis.Add(
                    "⚠ Qualidade da conexão: REGULAR."
                );
            }
            else
            {
                analysis.Add(
                    "✗ Qualidade da conexão: INSTÁVEL."
                );
            }

            return string.Join(
                Environment.NewLine,
                analysis
            );
        }
    }
}