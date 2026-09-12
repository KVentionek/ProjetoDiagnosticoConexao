using DiagnosticoConexao.Models;
using DiagnosticoConexao.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagnosticoConexao.WPF
{
    public partial class TracerouteWindow : Window
    {
        private readonly TracerouteService _tracerouteService;

        public TracerouteWindow()
        {
            InitializeComponent();

            _tracerouteService = new TracerouteService();
        }

        private async void ExecutarTraceroute_Click(
            object sender,
            RoutedEventArgs e)
        {
            string destination =
                TxtDestination.Text.Trim();

            if (string.IsNullOrWhiteSpace(destination))
            {
                MessageBox.Show(
                    "Informe um destino para executar o traceroute.",
                    "Traceroute",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            try
            {
                BtnTrace.IsEnabled = false;

                TxtTraceResult.Text =
                    $"Executando traceroute para {destination}...\n\n" +
                    "Aguarde enquanto os hops são identificados.";

                List<TracerouteHop> hops =
                    await _tracerouteService.TraceAsync(
                        destination
                    );

                if (hops.Count == 0)
                {
                    TxtTraceResult.Text =
                        "Nenhum resultado foi encontrado.";

                    return;
                }

                TxtTraceResult.Text =
                    BuildTraceResult(hops);
            }
            catch (Exception ex)
            {
                TxtTraceResult.Text =
                    $"Erro ao executar traceroute:\n\n{ex.Message}";
            }
            finally
            {
                BtnTrace.IsEnabled = true;
            }
        }

        private string BuildTraceResult(
            List<TracerouteHop> hops)
        {
            List<string> lines = new();

            foreach (TracerouteHop hop in hops)
            {
                if (hop.Success)
                {
                    lines.Add(
                        $"{hop.HopNumber,2}   " +
                        $"{hop.Address,-20} " +
                        $"{hop.Latency,4} ms"
                    );
                }
                else
                {
                    lines.Add(
                        $"{hop.HopNumber,2}   *"
                    );
                }
            }

            return string.Join(
                Environment.NewLine,
                lines
            );
        }
    }
}