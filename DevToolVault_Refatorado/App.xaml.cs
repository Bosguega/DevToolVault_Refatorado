// App.xaml.cs
using System.Windows;
using DevToolVault.Core.Models;
using DevToolVault.Core.Services;
using DevToolVault.Features.Filters;
using DevToolVault.Refatorado.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevToolVault
{
    public partial class App
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Serviços
            services.AddSingleton<FileFilterManager>();
            services.AddSingleton<FileStatistics>();
            services.AddSingleton<IAppNavigationService, AppNavigationService>();

            // Estratégias de exportação
            services.AddTransient<ITextExportStrategy, TextExportStrategy>();
            services.AddTransient<IMarkdownExportStrategy, MarkdownExportStrategy>();
            services.AddTransient<IPdfExportStrategy, PdfExportStrategy>();
            services.AddTransient<IZipExportStrategy, ZipExportStrategy>();
            services.AddSingleton<IExportService, ExportService>();

            // Janelas
            services.AddTransient<Features.Start.StartWindow>();
            services.AddTransient<Features.Structure.EstruturaWindow>();
            services.AddTransient<Features.Export.ExportarCodigoWindow>();
            services.AddTransient<Features.Filters.FilterManagerWindow>();
            services.AddTransient<Features.Filters.ProjectTypeSelectorWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<Features.Start.StartWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
