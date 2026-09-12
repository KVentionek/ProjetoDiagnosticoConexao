namespace DiagnosticoConexao.Models
{
    public class PortTestResult
    {
        public string? Host { get; set; }

        public int Port { get; set; }

        public bool Open { get; set; }

        public long Latency { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public int ProcessId { get; set; }

        public string? ProcessName { get; set; }
    }
}