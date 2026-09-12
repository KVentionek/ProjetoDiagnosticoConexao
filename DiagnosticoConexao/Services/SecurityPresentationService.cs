using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services;

public class SecurityPresentationService
{
    public void PrintSecurityAnalysis(
        List<SecurityFinding> findings,
        SecuritySummary summary)
    {
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("  ANÁLISE DE SEGURANÇA DE PORTAS");
        Console.WriteLine("=================================");
        Console.WriteLine();

        PrintSummary(summary);

        PrintAttentionItems(findings);

        PrintExternalAnalysis(findings);

        PrintLocalAnalysis(findings);
    }

    private void PrintSummary(SecuritySummary summary)
    {
        Console.WriteLine("RESUMO");
        Console.WriteLine();

        Console.WriteLine(
            $"✓ {summary.SafeCount} item(ns) seguro(s)"
        );

        Console.WriteLine(
            $"ℹ {summary.InformationCount} item(ns) informativo(s)"
        );

        Console.WriteLine(
            $"⚠ {summary.AttentionCount} item(ns) que merecem atenção"
        );

        if (summary.CriticalCount > 0)
        {
            Console.WriteLine(
                $"✗ {summary.CriticalCount} item(ns) crítico(s)"
            );
        }

        Console.WriteLine();

        Console.WriteLine(
            "⚠ Atenção indica que o item merece verificação."
        );

        Console.WriteLine(
            "  Não significa necessariamente uma vulnerabilidade."
        );

        Console.WriteLine();
    }

    private void PrintAttentionItems(
        List<SecurityFinding> findings)
    {
        bool hasAttentionItems =
            findings.Any(
                finding =>
                    finding.Severity == SecuritySeverity.Attention ||
                    finding.Severity == SecuritySeverity.Critical
            );

        if (!hasAttentionItems)
            return;

        Console.WriteLine("Itens que merecem atenção:");
        Console.WriteLine();

        foreach (SecurityFinding finding in findings)
        {
            if (finding.Severity != SecuritySeverity.Attention &&
                finding.Severity != SecuritySeverity.Critical)
            {
                continue;
            }

            string symbol =
                GetSeveritySymbol(finding.Severity);

            string process =
                string.IsNullOrWhiteSpace(finding.ProcessName)
                    ? "processo não identificado"
                    : finding.ProcessName;

            Console.WriteLine(
                $"{symbol} Porta {finding.Port} → {process}"
            );

            Console.WriteLine(
                $"   {finding.Title}"
            );

            Console.WriteLine();
        }
    }

    private void PrintExternalAnalysis(
        List<SecurityFinding> findings)
    {
        Console.WriteLine("---------------------------------");
        Console.WriteLine("ANÁLISE EXTERNA");
        Console.WriteLine("---------------------------------");
        Console.WriteLine();

        foreach (SecurityFinding finding in findings)
        {
            if (finding.Source != SecuritySource.External)
                continue;

            string symbol =
                GetSeveritySymbol(finding.Severity);

            Console.WriteLine(
                $"{symbol} Porta {finding.Port} → destino externo"
            );

            Console.WriteLine(
                $"→ {finding.Title}"
            );

            Console.WriteLine(
                $"→ {finding.Description}"
            );

            Console.WriteLine();
        }
    }

    private void PrintLocalAnalysis(
        List<SecurityFinding> findings)
    {
        Console.WriteLine("---------------------------------");
        Console.WriteLine("ANÁLISE LOCAL");
        Console.WriteLine("---------------------------------");
        Console.WriteLine();

        foreach (SecurityFinding finding in findings)
        {
            if (finding.Source != SecuritySource.Local)
                continue;

            string symbol =
                GetSeveritySymbol(finding.Severity);

            string process =
                string.IsNullOrWhiteSpace(finding.ProcessName)
                    ? "processo não identificado"
                    : finding.ProcessName;

            Console.WriteLine(
                $"{symbol} Porta {finding.Port} → {process}"
            );

            Console.WriteLine(
                $"→ {finding.Title}"
            );

            Console.WriteLine(
                $"→ {finding.Description}"
            );

            Console.WriteLine();
        }
    }

    private string GetSeveritySymbol(
        SecuritySeverity severity)
    {
        return severity switch
        {
            SecuritySeverity.Critical => "✗",
            SecuritySeverity.Attention => "⚠",
            SecuritySeverity.Information => "ℹ",
            SecuritySeverity.Safe => "✓",
            _ => "•"
        };
    }
}