using System.Diagnostics;
using System.Net.Sockets;
using DiagnosticoConexao.Models;

namespace DiagnosticoConexao.Services
{
    public class PortService
    {
        public async Task<PortTestResult> TestAsync(
            string host,
            int port,
            int timeoutMilliseconds = 1000)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using TcpClient client = new TcpClient();

                Task connectTask = client.ConnectAsync(host, port);

                Task completedTask =
                    await Task.WhenAny(
                        connectTask,
                        Task.Delay(timeoutMilliseconds)
                    );

                if (completedTask != connectTask)
                {
                    throw new SocketException(
                        (int)SocketError.TimedOut
                    );
                }

                await connectTask;

                stopwatch.Stop();

                return new PortTestResult
                {
                    Host = host,
                    Port = port,
                    Open = true,
                    Latency = stopwatch.ElapsedMilliseconds,
                    Status = "OPEN",
                    Description = "Conexão TCP estabelecida."
                };
            }
            catch (SocketException ex)
            {
                stopwatch.Stop();

                string status;
                string description;

                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    status = "REFUSED";

                    description =
                        "O destino recusou a conexão.";
                }
                else if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    status = "TIMEOUT";

                    description =
                        "A conexão expirou sem resposta.";
                }
                else if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    status = "HOST_NOT_FOUND";

                    description =
                        "O host não pôde ser encontrado.";
                }
                else
                {
                    status = "ERROR";

                    description =
                        $"Erro de conexão: {ex.SocketErrorCode}";
                }

                return new PortTestResult
                {
                    Host = host,
                    Port = port,
                    Open = false,
                    Latency = 0,
                    Status = status,
                    Description = description
                };
            }
        }
    }
}