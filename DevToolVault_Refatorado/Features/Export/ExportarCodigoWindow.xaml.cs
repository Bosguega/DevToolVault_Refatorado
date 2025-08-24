// Features/Export/ExportarCodigoWindow.xaml.cs
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevToolVault.Controls;
using DevToolVault.Core.Models;
using DevToolVault.Core.Services;
using DevToolVault.Filters;
using Ookii.Dialogs.Wpf;

namespace DevToolVault.Features.Export
{
    public partial class ExportarCodigoWindow : Window
    {
        private readonly FileFilterManager _filterManager;
        private readonly IExportService _exportService;
        private readonly IAppNavigationService _navigationService;
        private string _projectRoot;
        private bool _isProcessing;

        public ExportarCodigoWindow(FileFilterManager filterManager, IExportService exportService, IAppNavigationService navigationService)
        {
            InitializeComponent();

            _filterManager = filterManager;
            _exportService = exportService;
            _navigationService = navigationService;

            var activeProfile = _filterManager.GetActiveProfile();
            if (activeProfile != null)
            {
                fileTreeView.LoadDirectory(activeProfile.RootPath, _filterManager);
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this) == true)
            {
                txtFolderPath.Text = dialog.SelectedPath;
                _projectRoot = txtFolderPath.Text;
                fileTreeView.LoadDirectory(_projectRoot, _filterManager);
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            await ExportSelectedFilesAsync();
        }

        private async Task ExportSelectedFilesAsync()
        {
            var selectedItems = fileTreeView.GetSelectedItems();
            if (!selectedItems.Any())
            {
                MessageBox.Show("Selecione pelo menos um arquivo para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var format = cmbExportFormat.SelectedIndex switch
            {
                1 => ExportFormat.Markdown,
                2 => ExportFormat.Pdf,
                3 => ExportFormat.Zip,
                _ => ExportFormat.Text
            };

            var defaultName = format switch
            {
                ExportFormat.Markdown => "codigo.md",
                ExportFormat.Pdf => "codigo.pdf",
                ExportFormat.Zip => "codigo.zip",
                _ => "codigo.txt"
            };

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = format switch
                {
                    ExportFormat.Markdown => "Arquivo Markdown (*.md)|*.md",
                    ExportFormat.Pdf => "PDF (*.pdf)|*.pdf",
                    ExportFormat.Zip => "Arquivo ZIP (*.zip)|*.zip",
                    _ => "Arquivo de Texto (*.txt)|*.txt"
                },
                FileName = defaultName
            };

            if (dlg.ShowDialog(this) != true) return;

            _isProcessing = true;
            SetControlsEnabled(false);

            try
            {
                await _exportService.ExportAsync(selectedItems, dlg.FileName, format);
                MessageBox.Show($"Exportação concluída!\n{selectedItems.Count} arquivos salvos.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                SetControlsEnabled(true);
            }
        }

        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = fileTreeView.GetSelectedItems();
            if (!selectedItems.Any())
            {
                MessageBox.Show("Selecione pelo menos um arquivo para visualizar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var previewWindow = new Window
            {
                Title = "Pré-visualização da Seleção",
                Width = 700,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var textBox = new TextBox
            {
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                Padding = new Thickness(10)
            };

            var previewContent = new StringBuilder();
            previewContent.AppendLine($"Arquivos selecionados ({selectedItems.Count}):");
            previewContent.AppendLine(new string('=', 50));
            previewContent.AppendLine();

            foreach (var item in selectedItems.OrderBy(i => i.RelativePath))
            {
                previewContent.AppendLine($"- {item.RelativePath}");
            }

            textBox.Text = previewContent.ToString();
            previewWindow.Content = textBox;
            previewWindow.Show();
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnExport.IsEnabled = enabled && !_isProcessing;
            btnBrowse.IsEnabled = enabled;
            btnPreview.IsEnabled = enabled;
            cmbExportFormat.IsEnabled = enabled;
        }
    }
}
