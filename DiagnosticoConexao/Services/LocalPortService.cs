using System.Diagnostics;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class LocalPortService
    {
        public List<PortTestResult> GetListeningPorts()
        {
            List<PortTestResult> ports = new List<PortTestResult>();

            try
            {
                using Process process = new Process();

                process.StartInfo.FileName = "netstat";
                process.StartInfo.Arguments = "-ano -p tcp";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output =
                    process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                string[] lines =
                    output.Split(
                        Environment.NewLine,
                        StringSplitOptions.RemoveEmptyEntries
                    );

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (!trimmedLine.StartsWith("TCP"))
                        continue;

                    string[] parts =
                        trimmedLine.Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries
                        );

                    if (parts.Length < 5)
                        continue;

                    string localEndpoint = parts[1];
                    string state = parts[3];
                    string processIdText = parts[4];

                    if (!state.Equals(
                            "LISTENING",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!int.TryParse(
                            processIdText,
                            out int processId))
                    {
                        continue;
                    }

                    if (!TryParseEndpoint(
                            localEndpoint,
                            out string host,
                            out int port))
                    {
                        continue;
                    }

                    string? processName =
                        GetProcessName(processId);

                    ports.Add(
                        new PortTestResult
                        {
                            Host = host,
                            Port = port,
                            Open = true,
                            Status = "LISTENING",
                            ProcessId = processId,
                            ProcessName = processName
                        }
                    );
                }
            }
            catch
            {
                // Retorna a lista vazia caso
                // o netstat não possa ser executado.
            }

            return ports
                .OrderBy(port => port.Port)
                .ThenBy(port => port.Host)
                .ToList();
        }

        private string? GetProcessName(int processId)
        {
            if (processId == 0)
                return null;

            try
            {
                using Process process =
                    Process.GetProcessById(processId);

                return process.ProcessName;
            }
            catch
            {
                return "Acesso negado";
            }
        }

        private bool TryParseEndpoint(
            string value,
            out string host,
            out int port)
        {
            host = string.Empty;
            port = 0;

            value = value.Trim();

            if (value.StartsWith("[") &&
                value.Contains("]:"))
            {
                int closingBracket =
                    value.LastIndexOf("]:");

                if (closingBracket == -1)
                    return false;

                host =
                    value.Substring(
                        1,
                        closingBracket - 1
                    );

                string portText =
                    value[(closingBracket + 2)..];

                return int.TryParse(
                    portText,
                    out port
                );
            }

            int separator =
                value.LastIndexOf(':');

            if (separator == -1)
                return false;

            host =
                value[..separator];

            string portTextIpv4 =
                value[(separator + 1)..];

            return int.TryParse(
                portTextIpv4,
                out port
            );
        }
    }
}