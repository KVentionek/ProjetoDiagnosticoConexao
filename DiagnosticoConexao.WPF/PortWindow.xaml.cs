using DiagnosticoConexao.Models;
using DiagnosticoConexao.Services;
using System.Windows;

namespace DiagnosticoConexao.WPF
{
    public partial class PortWindow : Window
    {
        private readonly PortService _portService;

        public PortWindow()
        {
            InitializeComponent();

            _portService = new PortService();
        }

        private async void ExecutarTestePorta_Click(
            object sender,
            RoutedEventArgs e)
        {
            string host =
                TxtHost.Text.Trim();

            string portText =
                TxtPort.Text.Trim();

            if (string.IsNullOrWhiteSpace(host))
            {
                MessageBox.Show(
                    "Informe um host para realizar o teste.",
                    "Teste de portas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            if (!int.TryParse(
                    portText,
                    out int port) ||
                port < 1 ||
                port > 65535)
            {
                MessageBox.Show(
                    "Informe uma porta válida entre 1 e 65535.",
                    "Teste de portas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            try
            {
                BtnTestPort.IsEnabled = false;

                TxtPortResult.Text =
                    $"Testando {host}:{port}...\n\n" +
                    "Aguarde enquanto a conexão é verificada.";

                PortTestResult result =
                    await _portService.TestAsync(
                        host,
                        port
                    );

                TxtPortResult.Text =
                    BuildPortResult(result);
            }
            catch (Exception ex)
            {
                TxtPortResult.Text =
                    $"Erro ao executar o teste:\n\n{ex.Message}";
            }
            finally
            {
                BtnTestPort.IsEnabled = true;
            }
        }

        private string BuildPortResult(
            PortTestResult result)
        {
            List<string> lines = new List<string>();

            lines.Add(
                $"Host:      {result.Host}"
            );

            lines.Add(
                $"Porta:     {result.Port}"
            );

            lines.Add(
                $"Status:    {result.Status}"
            );

            if (result.Open)
            {
                lines.Add(
                    $"Latência:  {result.Latency} ms"
                );
            }

            lines.Add("");

            lines.Add(
                result.Description ?? "--"
            );

            return string.Join(
                Environment.NewLine,
                lines
            );
        }
    }
}