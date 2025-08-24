// Features/Start/StartWindow.xaml.cs
using System.Windows;
using System.Windows.Input;
using DevToolVault.Core.Services;
using DevToolVault.Filters;
using DevToolVault.Features.Filters;
using DevToolVault.Features.Structure;
using DevToolVault.Features.Export;

namespace DevToolVault.Features.Start
{
    public partial class StartWindow : Window
    {
        // Corrigido CS0191: Removido 'readonly'
        private FileFilterManager _filterManager;
        private readonly IAppNavigationService _navigationService;

        public StartWindow(
            FileFilterManager filterManager,
            IAppNavigationService navigationService)
        {
            InitializeComponent();
            _filterManager = filterManager;
            _navigationService = navigationService;
            UpdateFiltroAtual();
        }

        private void UpdateFiltroAtual()
        {
            var activeProfile = _filterManager.GetActiveProfile();
            if (activeProfile != null)
            {
                menuFiltroAtual.Header = $"Filtro Atual: {activeProfile.Name}";
            }
        }

        private void MenuSelecionarTipoProjeto_Click(object sender, RoutedEventArgs e)
        {
            var selectorWindow = new ProjectTypeSelectorWindow(_filterManager);
            selectorWindow.Owner = this;
            if (selectorWindow.ShowDialog() == true)
            {
                _filterManager.SetActiveProfile(selectorWindow.SelectedProfile);
                UpdateFiltroAtual();
                MessageBox.Show($"Tipo de projeto definido como: {selectorWindow.SelectedProfile.Name}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuVisualizarEstrutura_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.Show<EstruturaWindow>();
        }

        private void MenuExportarCodigo_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.Show<ExportarCodigoWindow>();
        }

        private void MenuSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuGerenciarFiltros_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new FilterManagerWindow(_filterManager);
            filterWindow.Owner = this;
            filterWindow.ShowDialog();
            UpdateFiltroAtual();
        }

        private void MenuRecarregarFiltros_Click(object sender, RoutedEventArgs e)
        {
            // Corrigido CS0191: Agora possível atribuir porque não é readonly
            _filterManager = new FileFilterManager();
            UpdateFiltroAtual();
            MessageBox.Show("Filtros recarregados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuSobre_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DevToolVault v1.0\n\nFerramentas de desenvolvimento em um só lugar.\n\nDesenvolvido por: Seu Nome", "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BorderEstrutura_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuVisualizarEstrutura_Click(sender, e);
        }

        private void BorderExportar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuExportarCodigo_Click(sender, e);
        }

        private void BtnEstrutura_Click(object sender, RoutedEventArgs e)
        {
            MenuVisualizarEstrutura_Click(sender, e);
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            MenuExportarCodigo_Click(sender, e);
        }
    }
}