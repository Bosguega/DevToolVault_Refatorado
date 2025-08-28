using System.Windows;

namespace DevToolVault.Features.Export
{
    public partial class ExportarCodigoWindow : Window
    {
        public ExportarCodigoWindow(ExportarCodigoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
