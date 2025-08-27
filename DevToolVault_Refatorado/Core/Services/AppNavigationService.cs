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
            // Verifica se é a EstruturaWindow para injetar o ViewModel
            if (typeof(T) == typeof(DevToolVault.Features.Structure.EstruturaWindow))
            {
                // Obtém o ViewModel do contêiner DI
                var viewModel = _serviceProvider.GetService<DevToolVault.Features.Structure.EstruturaViewModel>();

                // Cria a janela passando o ViewModel injetado
                // (Isso pressupõe que você modificou o construtor de EstruturaWindow para aceitar o ViewModel)
                var window = new DevToolVault.Features.Structure.EstruturaWindow(viewModel);

                // Define o Owner (opcional, mas bom para janelas modais ou posicionamento relativo)
                // Você pode passar o Owner de alguma forma, por exemplo, como parâmetro do método Show
                // ou obtendo a janela ativa: var owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                // if (owner != null) window.Owner = owner;

                window.Show();
                return; // Importante: retorna para não executar o código padrão abaixo
            }

            // Código padrão para outras janelas (se for o padrão usado)
            // Se outras janelas também usarem DI para construtores, este bloco também precisaria ser adaptado.
            // var window = _serviceProvider.GetService<T>();
            // window?.Show();
        }

        public void ShowDialog<T>() where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            window.ShowDialog();
        }
    }
}