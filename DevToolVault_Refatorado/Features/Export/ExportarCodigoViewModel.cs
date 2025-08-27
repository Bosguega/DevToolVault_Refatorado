using DevToolVault.Core.Commands;
using DevToolVault.Core.Models;
using DevToolVault.Core.Services;
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Refatorado.Core.Services;
using DevToolVault.Services;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DevToolVault.Features.Export
{
    public class ExportarCodigoViewModel : INotifyPropertyChanged
    {
        private readonly FileFilterManager _filterManager;
        private readonly IExportService _exportService;
        private readonly TreeGeneratorService _treeGenerator;

        private ObservableCollection<FileSystemItem> _fileSystemItems;
        private string _currentPath;
        private bool _isLoading;
        private FilterProfile _activeProfile;

        public ObservableCollection<FileSystemItem> FileSystemItems
        {
            get => _fileSystemItems;
            set => SetProperty(ref _fileSystemItems, value);
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                if (SetProperty(ref _currentPath, value))
                    RaiseCommandsCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                    RaiseCommandsCanExecuteChanged();
            }
        }

        public FilterProfile ActiveProfile
        {
            get => _activeProfile;
            set => SetProperty(ref _activeProfile, value);
        }

        // Comandos
        public ICommand SelectFolderCommand { get; }
        public ICommand ExportTextCommand { get; }
        public ICommand ExportPdfCommand { get; }
        public ICommand ExportZipCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExportarCodigoViewModel(FileFilterManager filterManager, IExportService exportService)
        {
            _filterManager = filterManager ?? throw new ArgumentNullException(nameof(filterManager));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
            _treeGenerator = new TreeGeneratorService(_filterManager);

            FileSystemItems = new ObservableCollection<FileSystemItem>();
            ActiveProfile = _filterManager.GetActiveProfile();

            // Inicializa comandos
            SelectFolderCommand = new RelayCommand(async () => await SelectFolderAsync(), () => !IsLoading);
            ExportTextCommand = new RelayCommand(async () => await ExportAsync(ExportFormat.Text), CanExport);
            ExportPdfCommand = new RelayCommand(async () => await ExportAsync(ExportFormat.Pdf), CanExport);
            ExportZipCommand = new RelayCommand(async () => await ExportAsync(ExportFormat.Zip), CanExport);
        }

        private bool CanExport()
        {
            return !IsLoading && !string.IsNullOrWhiteSpace(CurrentPath) && Directory.Exists(CurrentPath) && FileSystemItems.Any();
        }

        private void RaiseCommandsCanExecuteChanged()
        {
            // Atualiza todos os comandos dependentes
            //(SelectFolderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //(ExportTextCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //(ExportPdfCommand as RelayCommand)?.RaiseCanExecuteChanged();
           // (ExportZipCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private async Task SelectFolderAsync()
        {
            var dialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrWhiteSpace(CurrentPath) && Directory.Exists(CurrentPath))
            {
                dialog.SelectedPath = CurrentPath;
            }

            var ownerWindow = GetOwnerWindow();

            if (dialog.ShowDialog(ownerWindow) == true)
            {
                CurrentPath = dialog.SelectedPath;
                await LoadDirectoryAsync(CurrentPath);
            }
        }

        public async Task LoadDirectoryAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;

            IsLoading = true;
            try
            {
                // Task.Run é necessário aqui porque GenerateTree é CPU-bound
                var items = await Task.Run(() => _treeGenerator.GenerateTree(path));
                FileSystemItems = new ObservableCollection<FileSystemItem>(items);
            }
            catch (Exception ex)
            {
                var ownerWindow = GetOwnerWindow();
                MessageBox.Show(ownerWindow, $"Erro ao carregar diretório: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportAsync(ExportFormat format)
        {
            if (!CanExport()) return;

            var selectedItems = new List<FileSystemItem>();
            GetSelectedItemsRecursive(FileSystemItems, selectedItems);

            var ownerWindow = GetOwnerWindow();

            if (!selectedItems.Any())
            {
                MessageBox.Show(ownerWindow, "Nenhum item selecionado para exportação.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string filter = "";
            string defaultExt = "";

            switch (format)
            {
                case ExportFormat.Text:
                    filter = "Arquivo de Texto (*.txt)|*.txt";
                    defaultExt = ".txt";
                    break;
                case ExportFormat.Markdown:
                    filter = "Arquivo Markdown (*.md)|*.md";
                    defaultExt = ".md";
                    break;
                case ExportFormat.Pdf:
                    filter = "Documento PDF (*.pdf)|*.pdf";
                    defaultExt = ".pdf";
                    break;
                case ExportFormat.Zip:
                    filter = "Arquivo ZIP (*.zip)|*.zip";
                    defaultExt = ".zip";
                    break;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = filter,
                FileName = $"Export_{Path.GetFileName(CurrentPath)}{defaultExt}"
            };

            if (saveDialog.ShowDialog(ownerWindow) == true)
            {
                IsLoading = true;
                try
                {
                    await _exportService.ExportAsync(selectedItems, saveDialog.FileName, format);

                    MessageBox.Show(ownerWindow,
                        $"Exportação concluída com sucesso para: {saveDialog.FileName}",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ownerWindow,
                        $"Erro durante a exportação: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void GetSelectedItemsRecursive(IEnumerable<FileSystemItem> items, List<FileSystemItem> selectedItems)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (item.IsChecked == true)
                    selectedItems.Add(item);

                if (item.Children != null && item.Children.Any())
                    GetSelectedItemsRecursive(item.Children, selectedItems);
            }
        }

        private Window GetOwnerWindow()
        {
            return Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current?.MainWindow;
        }
    }
}
