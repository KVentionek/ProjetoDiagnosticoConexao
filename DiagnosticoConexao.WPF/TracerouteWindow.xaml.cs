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

                TraceResultsPanel.Children.Clear();

                TextBlock loading =
                    new TextBlock
                    {
                        Text =
                            $"Executando traceroute para {destination}...\n\n" +
                            "Aguarde enquanto os hops são identificados.",

                        Foreground =
                            new SolidColorBrush(
                                Color.FromRgb(162, 167, 181)
                            ),

                        FontSize = 14,

                        Margin =
                            new Thickness(0, 12, 0, 12)
                    };

                TraceResultsPanel.Children.Add(
                    loading
                );

                List<TracerouteHop> hops =
                    await _tracerouteService.TraceAsync(
                        destination
                    );

                if (hops.Count == 0)
                {
                    TraceResultsPanel.Children.Clear();

                    TextBlock emptyMessage =
                        new TextBlock
                        {
                            Text =
                                "Nenhum resultado foi encontrado.",

                            Foreground =
                                new SolidColorBrush(
                                    Color.FromRgb(162, 167, 181)
                                ),

                            FontSize = 14,

                            Margin =
                                new Thickness(0, 12, 0, 12)
                        };

                    TraceResultsPanel.Children.Add(
                        emptyMessage
                    );

                    return;
                }

                DisplayHops(hops);
            }
            catch (Exception ex)
            {
                TraceResultsPanel.Children.Clear();

                TextBlock errorMessage =
                    new TextBlock
                    {
                        Text =
                            $"Erro ao executar traceroute:\n\n{ex.Message}",

                        Foreground =
                            new SolidColorBrush(
                                Color.FromRgb(235, 87, 87)
                            ),

                        FontSize = 14,

                        TextWrapping = TextWrapping.Wrap,

                        Margin =
                            new Thickness(0, 12, 0, 12)
                    };

                TraceResultsPanel.Children.Add(
                    errorMessage
                );
            }
            finally
            {
                BtnTrace.IsEnabled = true;
            }
        }


        private void DisplayHops(
            List<TracerouteHop> hops)
        {
            TraceResultsPanel.Children.Clear();

            foreach (TracerouteHop hop in hops)
            {
                Grid grid =
                    new Grid();

                // EXATAMENTE AS MESMAS COLUNAS DO CABEÇALHO

                grid.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = new GridLength(90)
                    }
                );

                grid.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(
                                1,
                                GridUnitType.Star
                            )
                    }
                );

                grid.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = new GridLength(150)
                    }
                );


                string address =
                    hop.Success
                        ? hop.Address ?? "--"
                        : "*";

                string latency =
                    hop.Success
                        ? $"{hop.Latency} ms"
                        : "--";


                AddCell(
                    grid,
                    hop.HopNumber.ToString(),
                    0,
                    hop.Success
                );

                AddCell(
                    grid,
                    address,
                    1,
                    hop.Success
                );

                AddCell(
                    grid,
                    latency,
                    2,
                    hop.Success
                );


                Border row =
                    new Border
                    {
                        Background =
                            new SolidColorBrush(
                                Color.FromRgb(
                                    25,
                                    28,
                                    35
                                )
                            ),

                        BorderBrush =
                            new SolidColorBrush(
                                Color.FromRgb(
                                    39,
                                    43,
                                    53
                                )
                            ),

                        BorderThickness =
                            new Thickness(
                                0,
                                0,
                                0,
                                1
                            ),

                        Padding =
                            new Thickness(
                                12,
                                14,
                                12,
                                14
                            ),

                        Child = grid
                    };


                TraceResultsPanel.Children.Add(
                    row
                );
            }
        }


        private void AddCell(
            Grid grid,
            string text,
            int column,
            bool success)
        {
            TextBlock cell =
                new TextBlock
                {
                    Text = text,

                    Foreground =
                        success
                            ? new SolidColorBrush(
                                Color.FromRgb(
                                    213,
                                    215,
                                    220
                                )
                            )
                            : new SolidColorBrush(
                                Color.FromRgb(
                                    111,
                                    116,
                                    130
                                )
                            ),

                    FontSize = 13,

                    TextWrapping =
                        TextWrapping.Wrap,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Margin =
                        new Thickness(
                            0,
                            0,
                            12,
                            0
                        )
                };


            Grid.SetColumn(
                cell,
                column
            );

            grid.Children.Add(
                cell
            );
        }

        private TextBlock CreateCell(
            string text,
            bool success)
        {
            return new TextBlock
            {
                Text = text,

                Foreground =
                    success
                        ? new SolidColorBrush(
                            Color.FromRgb(
                                213,
                                215,
                                220
                            )
                        )
                        : new SolidColorBrush(
                            Color.FromRgb(
                                111,
                                116,
                                130
                            )
                        ),

                FontSize = 13,

                FontWeight =
                    success
                        ? FontWeights.Normal
                        : FontWeights.Normal,

                VerticalAlignment =
                    VerticalAlignment.Center,

                TextWrapping =
                    TextWrapping.NoWrap,

                Margin =
                    new Thickness(
                        0,
                        0,
                        12,
                        0
                    )
            };
        }
    }
}