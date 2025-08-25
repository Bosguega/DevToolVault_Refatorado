// DevToolVault_Refatorado/Features/Export/ExportarCodigoWindow.xaml.cs
// Features/Export/ExportarCodigoWindow.xaml.cs
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevToolVault.Controls;
using DevToolVault.Core.Models;
using DevToolVault.Core.Services;
using DevToolVault.Features.Filters;
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Refatorado.Core.Services;
using DevToolVault.ViewModels;
using Ookii.Dialogs.Wpf;

namespace DevToolVault.Features.Export // Ensure namespace matches x:Class
{
    public partial class ExportarCodigoWindow : Window
    {
        private readonly ExportarCodigoViewModel _viewModel;

        public ExportarCodigoWindow(FileFilterManager filterManager)
        {
            InitializeComponent(); // This should work now if XAML is correct
            _viewModel = new ExportarCodigoViewModel(filterManager);
            DataContext = _viewModel;
        }

        private async void SelectDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this) == true)
            {
                await _viewModel.LoadDirectoryAsync(dialog.SelectedPath);
            }
        }

        private async void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is FilterProfile profile)
            {
                await _viewModel.SetActiveProfile(profile);
            }
        }
    }
}