using System.Net;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class DnsService
    {
        public async Task<DnsResult> ResolveAsync(string hostname)
        {
            try
            {
                IPAddress[] addresses =
                    await Dns.GetHostAddressesAsync(hostname);

                if (addresses.Length > 0)
                {
                    return new DnsResult
                    {
                        Success = true,
                        Hostname = hostname,
                        Address = addresses[0].ToString(),
                        Status = "Success"
                    };
                }

                return new DnsResult
                {
                    Success = false,
                    Hostname = hostname,
                    Status = "NoAddress"
                };
            }
            catch (Exception ex)
            {
                return new DnsResult
                {
                    Success = false,
                    Hostname = hostname,
                    Status = ex.Message
                };
            }
        }
    }
}