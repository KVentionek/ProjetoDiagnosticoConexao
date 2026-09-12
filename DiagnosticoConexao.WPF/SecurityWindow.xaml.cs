using DiagnosticoConexao.Models;
using DiagnosticoConexao.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagnosticoConexao.WPF
{
    public partial class SecurityWindow : Window
    {
        private readonly LocalPortService _localPortService;
        private readonly SecurityAnalysisService _securityAnalysisService;

        public SecurityWindow()
        {
            InitializeComponent();

            _localPortService = new LocalPortService();
            _securityAnalysisService = new SecurityAnalysisService();

            Loaded += SecurityWindow_Loaded;
        }

        private void SecurityWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            AtualizarAnalise();
        }

        private void AtualizarAnalise_Click(
            object sender,
            RoutedEventArgs e)
        {
            AtualizarAnalise();
        }

        private void AtualizarAnalise()
        {
            try
            {
                BtnAnalyze.IsEnabled = false;

                List<PortTestResult> localPorts =
                    _localPortService.GetListeningPorts();

                List<SecurityFinding> findings =
                    _securityAnalysisService.AnalyzeLocalPorts(
                        localPorts
                    );

                SecuritySummary summary =
                    _securityAnalysisService.GenerateSummary(
                        findings
                    );

                TxtSafeCount.Text =
                    summary.SafeCount.ToString();

                TxtInformationCount.Text =
                    summary.InformationCount.ToString();

                TxtAttentionCount.Text =
                    summary.AttentionCount.ToString();

                TxtCriticalCount.Text =
                    summary.CriticalCount.ToString();

                DisplayFindings(findings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível realizar a análise de segurança.\n\n{ex.Message}",
                    "Análise de segurança",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                BtnAnalyze.IsEnabled = true;
            }
        }

        private void DisplayFindings(
            List<SecurityFinding> findings)
        {
            PortResultsPanel.Children.Clear();

            foreach (SecurityFinding finding in findings)
            {
                Border row =
                    CreateFindingRow(finding);

                PortResultsPanel.Children.Add(row);
            }

            if (findings.Count == 0)
            {
                TextBlock emptyMessage =
                    new TextBlock
                    {
                        Text = "Nenhum serviço de rede foi encontrado.",
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(146, 151, 163)
                        ),
                        FontSize = 14,
                        Margin = new Thickness(4, 10, 4, 10)
                    };

                PortResultsPanel.Children.Add(
                    emptyMessage
                );
            }
        }

        private Border CreateFindingRow(
            SecurityFinding finding)
        {
            Grid grid =
                new Grid();

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(90)
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(180)
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(150)
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(140)
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                }
            );

            AddCell(
                grid,
                finding.Port.ToString(),
                0
            );

            AddCell(
                grid,
                finding.Host ?? "--",
                1
            );

            AddCell(
                grid,
                finding.ProcessName ?? "--",
                2
            );

            TextBlock severity =
                new TextBlock
                {
                    Text = finding.Severity.ToString(),
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment =
                        VerticalAlignment.Center
                };

            severity.Foreground =
                GetSeverityBrush(
                    finding.Severity
                );

            Grid.SetColumn(
                severity,
                3
            );

            grid.Children.Add(severity);

            AddCell(
                grid,
                finding.Description ?? "--",
                4
            );

            Border row =
                new Border
                {
                    Background =
                        new SolidColorBrush(
                            Color.FromRgb(25, 28, 35)
                        ),
                    BorderBrush =
                        new SolidColorBrush(
                            Color.FromRgb(39, 43, 53)
                        ),
                    BorderThickness =
                        new Thickness(0, 0, 0, 1),
                    Padding =
                        new Thickness(12, 14, 12, 14),
                    Child = grid
                };

            return row;
        }

        private void AddCell(
            Grid grid,
            string text,
            int column)
        {
            TextBlock cell =
                new TextBlock
                {
                    Text = text,
                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(213, 215, 220)
                        ),
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment =
                        VerticalAlignment.Center,
                    Margin =
                        new Thickness(0, 0, 12, 0)
                };

            Grid.SetColumn(
                cell,
                column
            );

            grid.Children.Add(cell);
        }

        private Brush GetSeverityBrush(
            SecuritySeverity severity)
        {
            return severity switch
            {
                SecuritySeverity.Safe =>
                    new SolidColorBrush(
                        Color.FromRgb(93, 211, 158)
                    ),

                SecuritySeverity.Information =>
                    new SolidColorBrush(
                        Color.FromRgb(111, 168, 220)
                    ),

                SecuritySeverity.Attention =>
                    new SolidColorBrush(
                        Color.FromRgb(240, 179, 90)
                    ),

                SecuritySeverity.Critical =>
                    new SolidColorBrush(
                        Color.FromRgb(235, 87, 87)
                    ),

                _ =>
                    new SolidColorBrush(
                        Color.FromRgb(213, 215, 220)
                    )
            };
        }
    }
}