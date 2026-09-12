namespace DiagnosticoConexao.Models
{
    public class TracerouteHop
    {
        public int HopNumber { get; set; }

        public string? Address { get; set; }

        public long Latency { get; set; }

        public bool Success { get; set; }
    }
}