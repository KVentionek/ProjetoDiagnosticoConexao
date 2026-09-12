using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagnosticoConexao.Models
{
    public class NetworkInfo
    {
        public string? Name { get; set; }

        public string? Type { get; set; }

        public string? IPv4 { get; set; }

        public string? SubnetMask { get; set; }

        public int Cidr { get; set; }

        public string? NetworkAddress { get; set; }

        public List<string> DnsServers { get; set; } = new();

        public string? Gateway { get; set; }
    }
}