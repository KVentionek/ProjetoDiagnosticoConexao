namespace DiagnosticoConexao.Models
{
    public class SecurityFinding
    {
        public int Port { get; set; }

        public string? Host { get; set; }

        public string? ProcessName { get; set; }

        public SecuritySource Source { get; set; }

        public SecuritySeverity Severity { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}