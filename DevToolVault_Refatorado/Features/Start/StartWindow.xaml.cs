using System.Windows;
using DevToolVault.Core.Services;
using DevToolVault.Refatorado.Core.Services;

namespace DevToolVault.Features.Start
{
    public partial class StartWindow : Window
    {
        public StartWindow(FileFilterManager filterManager, IAppNavigationService navigationService)
        {
            InitializeComponent();
            DataContext = new StartWindowViewModel(filterManager, navigationService);
        }
    }
}
