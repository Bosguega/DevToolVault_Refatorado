// Features/Filters/ProjectTypeSelectorWindow.xaml.cs
using System.Windows;
using DevToolVault.Features.Filters;
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Refatorado.Core.Services;

namespace DevToolVault.Features.Filters
{
    public partial class ProjectTypeSelectorWindow : Window
    {
        public FilterProfile SelectedProfile { get; private set; }
        private readonly FileFilterManager _filterManager;

        public ProjectTypeSelectorWindow(FileFilterManager filterManager)
        {
            InitializeComponent();
            _filterManager = filterManager;
            lstProfiles.ItemsSource = _filterManager.GetProfiles();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lstProfiles.SelectedItem is FilterProfile profile)
            {
                SelectedProfile = profile;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Selecione um tipo de projeto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}