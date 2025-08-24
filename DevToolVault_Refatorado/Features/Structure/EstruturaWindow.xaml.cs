// Features/Structure/EstruturaWindow.xaml.cs
using System.Linq;
using System.Windows;
using DevToolVault.Filters;
using DevToolVault.Controls;

namespace DevToolVault.Features.Structure
{
    public partial class EstruturaWindow : Window
    {
        private readonly FileFilterManager _filterManager;

        public EstruturaWindow(FileFilterManager filterManager)
        {
            InitializeComponent();
            _filterManager = filterManager;

            // Carrega a raiz do projeto, se houver
            var activeProfile = _filterManager.GetActiveProfile();
            if (activeProfile != null)
            {
                fileTreeView.LoadDirectory(activeProfile.RootPath, _filterManager);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_filterManager.GetActiveProfile() != null)
                fileTreeView.LoadDirectory(_filterManager.GetActiveProfile().RootPath, _filterManager);
        }

        private void BtnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            fileTreeView.SetAllExpanded(true);
        }

        private void BtnCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            fileTreeView.SetAllExpanded(false);
        }

        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            fileTreeView.SetAllItemsChecked(true);
        }

        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            fileTreeView.SetAllItemsChecked(false);
        }
    }
}
