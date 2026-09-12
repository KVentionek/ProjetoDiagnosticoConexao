namespace DiagnosticoConexao.Models
{
    public class SecuritySummary
    {
        public int SafeCount { get; set; }

        public int InformationCount { get; set; }

        public int AttentionCount { get; set; }

        public int CriticalCount { get; set; }
    }
}