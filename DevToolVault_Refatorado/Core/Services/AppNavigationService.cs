// Core/Services/AppNavigationService.cs
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DevToolVault.Core.Services
{
    public class AppNavigationService : IAppNavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show<T>() where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            window.Show();
        }

        public void ShowDialog<T>() where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            window.ShowDialog();
        }
    }
}