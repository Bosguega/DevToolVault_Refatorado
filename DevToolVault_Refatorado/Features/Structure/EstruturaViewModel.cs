using DevToolVault.Core.Models;
using DevToolVault.Refatorado.Core.Services;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevToolVault.Core.Commands;
using DevToolVault.Services;

namespace DevToolVault.Features.Structure
{
    public class EstruturaViewModel : INotifyPropertyChanged
    {
        private readonly FileFilterManager _filterManager;
        private string _selectedPath;
        private string _structureText;
        private bool _isGenerating;
        private string _currentProfileName;

        public string SelectedPath
        {
            get => _selectedPath;
            set
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StructureText
        {
            get => _structureText;
            set
            {
                if (_structureText != value)
                {
                    _structureText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                if (_isGenerating != value)
                {
                    _isGenerating = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentProfileName
        {
            get => _currentProfileName;
            set
            {
                if (_currentProfileName != value)
                {
                    _currentProfileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand BrowseFolderCommand { get; }
        public ICommand GenerateStructureCommand { get; }
        public ICommand SaveStructureCommand { get; }
        public ICommand CopyStructureCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EstruturaViewModel(FileFilterManager filterManager)
        {
            _filterManager = filterManager ?? throw new ArgumentNullException(nameof(filterManager));
            UpdateCurrentProfileName();

            BrowseFolderCommand = new RelayCommand(() => BrowseFolder(), () => !IsGenerating);
            GenerateStructureCommand = new RelayCommand(async () => await GenerateStructureAsync(),
                () => !IsGenerating && !string.IsNullOrWhiteSpace(SelectedPath) && Directory.Exists(SelectedPath));
            SaveStructureCommand = new RelayCommand(SaveStructure, () => !IsGenerating && !string.IsNullOrWhiteSpace(StructureText));
            CopyStructureCommand = new RelayCommand(CopyStructure, () => !IsGenerating && !string.IsNullOrWhiteSpace(StructureText));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ---------- Métodos ----------

        private void BrowseFolder()
        {
            var dialog = new VistaFolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(SelectedPath) && Directory.Exists(SelectedPath))
            {
                dialog.SelectedPath = SelectedPath;
            }

            var ownerWindow = System.Windows.Application.Current.Windows
                .OfType<System.Windows.Window>()
                .SingleOrDefault(x => x.IsActive) ?? System.Windows.Application.Current.MainWindow;

            if (dialog.ShowDialog(ownerWindow) == true)
            {
                SelectedPath = dialog.SelectedPath;
            }
        }

        private async Task GenerateStructureAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedPath) || !Directory.Exists(SelectedPath))
            {
                StructureText = "Por favor, selecione um diretório válido.";
                return;
            }

            IsGenerating = true;
            StructureText = "Gerando estrutura...";
            UpdateCurrentProfileName();

            try
            {
                var treeGenerator = new TreeGeneratorService(_filterManager);
                var fileSystemItems = await Task.Run(() => treeGenerator.GenerateTree(SelectedPath));

                var sb = new StringBuilder();
                sb.AppendLine($". ({Path.GetFileName(SelectedPath)})");
                AppendItemsToStringBuilder(sb, fileSystemItems, "", true);

                StructureText = sb.ToString();
            }
            catch (UnauthorizedAccessException ex)
            {
                StructureText = $"Acesso negado ao diretório: {ex.Message}";
            }
            catch (Exception ex)
            {
                StructureText = $"Erro ao gerar estrutura: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void AppendItemsToStringBuilder(StringBuilder sb, IEnumerable<FileSystemItem> items, string indent, bool isLast)
        {
            if (items == null) return;

            var itemList = items.ToList();
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                bool isLastItem = (i == itemList.Count - 1);
                string prefix = isLastItem ? "└── " : "├── ";
                string childIndent = isLastItem ? "    " : "│   ";

                sb.AppendLine($"{indent}{prefix}{item.Name}{(item.IsDirectory ? "/" : "")}");

                if (item.Children != null && item.Children.Any())
                {
                    AppendItemsToStringBuilder(sb, item.Children, indent + childIndent, isLastItem);
                }
            }
        }

        private void SaveStructure()
        {
            if (string.IsNullOrWhiteSpace(StructureText)) return;

            var saveDialog = new SaveFileDialog
            {
                Filter = "Arquivo de Texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*",
                FileName = $"Estrutura_{Path.GetFileName(SelectedPath)}.txt"
            };

            var ownerWindow = System.Windows.Application.Current.Windows
                .OfType<System.Windows.Window>()
                .SingleOrDefault(x => x.IsActive) ?? System.Windows.Application.Current.MainWindow;

            if (saveDialog.ShowDialog(ownerWindow) == true)
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, StructureText);
                    System.Windows.MessageBox.Show("Arquivo salvo com sucesso.", "Sucesso",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erro ao salvar arquivo: {ex.Message}", "Erro",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void CopyStructure()
        {
            if (!string.IsNullOrWhiteSpace(StructureText))
            {
                try
                {
                    System.Windows.Clipboard.SetText(StructureText);
                    System.Windows.MessageBox.Show("Estrutura copiada para a área de transferência.", "Sucesso",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erro ao copiar: {ex.Message}", "Erro",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void UpdateCurrentProfileName()
        {
            var activeProfile = _filterManager.GetActiveProfile();
            CurrentProfileName = activeProfile?.Name ?? "Nenhum";
        }
    }
}
