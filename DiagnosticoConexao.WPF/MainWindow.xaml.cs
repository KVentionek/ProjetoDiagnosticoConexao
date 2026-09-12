using DiagnosticoConexao.Models;
using DiagnosticoConexao.Services;
using System.Windows;
using System.Windows.Controls;

namespace DiagnosticoConexao.WPF
{
    public partial class MainWindow : Window
    {
        private readonly DiagnosticService _diagnosticService;
        private readonly AnalysisService _analysisService;
        private readonly LocalPortService _localPortService;
        private readonly SecurityAnalysisService _securityAnalysisService;

        public MainWindow()
        {
            InitializeComponent();

            _diagnosticService = new DiagnosticService();
            _analysisService = new AnalysisService();
            _localPortService = new LocalPortService();
            _securityAnalysisService = new SecurityAnalysisService();
        }

        private async void ExecutarDiagnostico_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button? button = sender as Button;

            try
            {
                if (button != null)
                    button.IsEnabled = false;

                // Estado inicial durante o diagnóstico
                TxtInternet.Text = "TESTANDO...";
                TxtInternet.Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(240, 179, 90)
                    );

                TxtLatency.Text = "--";
                TxtPacketLoss.Text = "--";

                NetworkDiagnosticResult? result =
                    await _diagnosticService.DiagnoseAsync();

                if (result == null)
                {
                    TxtInternet.Text = "SEM REDE";

                    return;
                }

                // INFORMAÇÕES DA REDE

                if (result.Network != null)
                {
                    TxtInterface.Text =
                        result.Network.Name ?? "--";

                    TxtNetworkType.Text =
                        result.Network.Type ?? "--";

                    TxtIPv4.Text =
                        result.Network.IPv4 ?? "--";

                    TxtSubnetMask.Text =
                        result.Network.SubnetMask ?? "--";

                    TxtCidr.Text =
                        $"/{result.Network.Cidr}";

                    TxtNetworkAddress.Text =
                        result.Network.NetworkAddress ?? "--";

                    TxtGateway.Text =
                        result.Network.Gateway ?? "--";

                    TxtDns.Text =
                        result.Network.DnsServers.Count > 0
                            ? string.Join(", ", result.Network.DnsServers)
                            : "--";
                }

                // ANÁLISE DA CONEXÃO

                string analysis =
                    _analysisService.Analyze(result);

                TxtAnalysis.Text = analysis;

                // INTERNET
                if (result.InternetAvailable)
                {
                    TxtInternet.Text = "ONLINE";

                    TxtInternet.Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(93, 211, 158)
                        );
                }
                else
                {
                    TxtInternet.Text = "OFFLINE";

                    TxtInternet.Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(235, 87, 87)
                        );
                }

                // LATÊNCIA
                TxtLatency.Text =
                    $"{result.InternetAverageLatency:F1} ms";

                // PERDA DE PACOTES
                TxtPacketLoss.Text =
                    $"{result.InternetPacketLossPercentage:F1}%";

                List<PortTestResult> localPorts =
                    _localPortService.GetListeningPorts();

                List<SecurityFinding> securityFindings =
                    _securityAnalysisService.AnalyzeLocalPorts(
                        localPorts
                    );

                SecuritySummary securitySummary =
                    _securityAnalysisService.GenerateSummary(
                        securityFindings
                    );

                TxtSecurity.Text =
                    $"{securitySummary.AttentionCount} ⚠";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocorreu um erro durante o diagnóstico:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                if (button != null)
                    button.IsEnabled = true;
            }
        }

        private void AbrirTraceroute_Click(
            object sender,
            RoutedEventArgs e)
        {
            TracerouteWindow window =
                new TracerouteWindow();

            window.Owner = this;

            window.ShowDialog();
        }

        private void AbrirTestePortas_Click(
            object sender,
            RoutedEventArgs e)
        {
            PortWindow window =
                new PortWindow();

            window.Owner = this;

            window.ShowDialog();
        }

        private void AbrirSeguranca_Click(
            object sender,
            RoutedEventArgs e)
        {
            SecurityWindow window =
                new SecurityWindow();

            window.Owner = this;

            window.ShowDialog();
        }
    }
}