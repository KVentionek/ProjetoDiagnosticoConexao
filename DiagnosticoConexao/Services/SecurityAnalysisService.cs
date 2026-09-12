using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class SecurityAnalysisService
    {
        public List<SecurityFinding> AnalyzePorts(
            List<PortTestResult> portTests)
        {
            List<SecurityFinding> findings = new List<SecurityFinding>();

            foreach (PortTestResult port in portTests)
            {
                if (!port.Open)
                    continue;

                switch (port.Port)
                {
                    case 22:
                        findings.Add(
                            new SecurityFinding
                            {
                                Port = port.Port,
                                Host = port.Host,
                                ProcessName = port.ProcessName,
                                Source = SecuritySource.External,
                                Severity = SecuritySeverity.Attention,
                                Title = "SSH acessível",
                                Description =
                                    "Serviço de acesso remoto detectado. Verifique se a exposição é necessária."
                            }
                        );
                        break;

                    case 3389:
                        findings.Add(
                            new SecurityFinding
                            {
                                Port = port.Port,
                                Host = port.Host,
                                ProcessName = port.ProcessName,
                                Source = SecuritySource.External,
                                Severity = SecuritySeverity.Attention,
                                Title = "RDP acessível",
                                Description =
                                    "Serviço de acesso remoto do Windows detectado. Verifique se a exposição é necessária."
                            }
                        );
                        break;

                    case 80:
                        findings.Add(
                            new SecurityFinding
                            {
                                Port = port.Port,
                                Host = port.Host,
                                ProcessName = port.ProcessName,
                                Source = SecuritySource.External,
                                Severity = SecuritySeverity.Information,
                                Title = "HTTP acessível",
                                Description =
                                    "A porta HTTP está acessível no destino analisado."
                            }
                        );
                        break;

                    case 443:
                        findings.Add(
                            new SecurityFinding
                            {
                                Port = port.Port,
                                Host = port.Host,
                                ProcessName = port.ProcessName,
                                Source = SecuritySource.External,
                                Severity = SecuritySeverity.Safe,
                                Title = "HTTPS acessível",
                                Description =
                                    "A porta HTTPS está acessível no destino analisado."
                            }
                        );
                        break;

                    default:
                        findings.Add(
                            new SecurityFinding
                            {
                                Port = port.Port,
                                Host = port.Host,
                                ProcessName = port.ProcessName,
                                Source = SecuritySource.External,
                                Severity = SecuritySeverity.Information,
                                Title = "Porta acessível",
                                Description =
                                    "A porta TCP está acessível no destino analisado."
                            }
                        );
                        break;
                }
            }

            return findings;
        }

        public List<SecurityFinding> AnalyzeLocalPorts(
            List<PortTestResult> localPorts)
        {
            List<SecurityFinding> findings = new List<SecurityFinding>();

            foreach (PortTestResult port in localPorts)
            {
                bool isLocalhost =
                    port.Host == "127.0.0.1" ||
                    port.Host == "::1";

                bool isAllInterfaces =
                    port.Host == "0.0.0.0";

                string process =
                    string.IsNullOrWhiteSpace(port.ProcessName)
                        ? "processo não identificado"
                        : port.ProcessName;

                // ---------------------------------------------
                // SOMENTE O COMPUTADOR LOCAL
                // ---------------------------------------------

                if (isLocalhost)
                {
                    findings.Add(
                        new SecurityFinding
                        {
                            Port = port.Port,
                            Host = port.Host,
                            ProcessName = process,
                            Source = SecuritySource.Local,
                            Severity = SecuritySeverity.Safe,
                            Title = "Serviço local",
                            Description =
                                $"O processo está escutando somente em {port.Host} e não está exposto diretamente à rede."
                        }
                    );

                    continue;
                }

                // ---------------------------------------------
                // ACESSO REMOTO
                // ---------------------------------------------

                if (process.Equals(
                        "AnyDesk",
                        StringComparison.OrdinalIgnoreCase))
                {
                    findings.Add(
                        new SecurityFinding
                        {
                            Port = port.Port,
                            Host = port.Host,
                            ProcessName = process,
                            Source = SecuritySource.Local,
                            Severity = SecuritySeverity.Information,
                            Title = "Serviço de acesso remoto",
                            Description =
                                $"O AnyDesk está escutando em {port.Host}. A exposição pode ser esperada dependendo da configuração."
                        }
                    );

                    continue;
                }

                // ---------------------------------------------
                // PROCESSOS DO WINDOWS
                // ---------------------------------------------

                if (process.Equals(
                        "lsass",
                        StringComparison.OrdinalIgnoreCase) ||
                    process.Equals(
                        "wininit",
                        StringComparison.OrdinalIgnoreCase) ||
                    process.Equals(
                        "services",
                        StringComparison.OrdinalIgnoreCase) ||
                    process.Equals(
                        "svchost",
                        StringComparison.OrdinalIgnoreCase) ||
                    process.Equals(
                        "spoolsv",
                        StringComparison.OrdinalIgnoreCase))
                {
                    findings.Add(
                        new SecurityFinding
                        {
                            Port = port.Port,
                            Host = port.Host,
                            ProcessName = process,
                            Source = SecuritySource.Local,
                            Severity = SecuritySeverity.Information,
                            Title = "Processo do Windows",
                            Description =
                                $"O processo do Windows está escutando em {port.Host}. A presença da porta, isoladamente, não indica uma vulnerabilidade."
                        }
                    );

                    continue;
                }

                // ---------------------------------------------
                // SMB / NETBIOS
                // ---------------------------------------------

                if (port.Port == 139 ||
                    port.Port == 445)
                {
                    findings.Add(
                        new SecurityFinding
                        {
                            Port = port.Port,
                            Host = port.Host,
                            ProcessName = process,
                            Source = SecuritySource.Local,
                            Severity = SecuritySeverity.Attention,
                            Title = "Serviço de compartilhamento",
                            Description =
                                "Porta associada à comunicação/compartilhamento de arquivos do Windows. Verifique as regras do firewall e se o recurso é necessário."
                        }
                    );

                    continue;
                }

                // ---------------------------------------------
                // TODAS AS INTERFACES
                // ---------------------------------------------

                if (isAllInterfaces)
                {
                    findings.Add(
                        new SecurityFinding
                        {
                            Port = port.Port,
                            Host = port.Host,
                            ProcessName = process,
                            Source = SecuritySource.Local,
                            Severity = SecuritySeverity.Attention,
                            Title = "Serviço exposto na rede",
                            Description =
                                "O processo está escutando em todas as interfaces IPv4. Verifique se o serviço precisa aceitar conexões pela rede."
                        }
                    );

                    continue;
                }

                // ---------------------------------------------
                // ENDEREÇO DE REDE ESPECÍFICO
                // ---------------------------------------------

                findings.Add(
                    new SecurityFinding
                    {
                        Port = port.Port,
                        Host = port.Host,
                        ProcessName = process,
                        Source = SecuritySource.Local,
                        Severity = SecuritySeverity.Information,
                        Title = "Serviço associado a endereço de rede",
                        Description =
                            $"O processo está escutando no endereço {port.Host}. Verifique se essa exposição na rede é necessária."
                    }
                );
            }

            return findings;
        }

        public SecuritySummary GenerateSummary(
            List<SecurityFinding> findings)
        {
            return new SecuritySummary
            {
                SafeCount = findings.Count(
                    finding => finding.Severity == SecuritySeverity.Safe
                ),

                InformationCount = findings.Count(
                    finding => finding.Severity == SecuritySeverity.Information
                ),

                AttentionCount = findings.Count(
                    finding => finding.Severity == SecuritySeverity.Attention
                ),

                CriticalCount = findings.Count(
                    finding => finding.Severity == SecuritySeverity.Critical
                )
            };
        }
    }
}