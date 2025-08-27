using System.Windows;

namespace DevToolVault.Core.Services
{
    public interface IAppNavigationService
    {
        void Show<T>() where T : Window;
        void ShowDialog<T>() where T : Window;
    }
}
