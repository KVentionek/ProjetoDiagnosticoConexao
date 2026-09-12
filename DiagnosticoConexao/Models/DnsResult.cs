using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagnosticoConexao.Models
{
    public class DnsResult
    {
        public bool Success { get; set; }

        public string? Hostname { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }
    }
}