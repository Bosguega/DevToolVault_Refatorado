// DevToolVault_Refatorado/Features/Structure/EstruturaViewModel.cs
using DevToolVault.Core.Models; // Assumindo que FilterProfile está aqui
using DevToolVault.Refatorado.Core.Services; // Assumindo que FileFilterManager está aqui
using Microsoft.Win32; // Para SaveFileDialog
using Ookii.Dialogs.Wpf; // Para VistaFolderBrowserDialog
using System;
using System.Collections.Generic; // Para List
using System.ComponentModel;
using System.IO;
using System.Linq; // Para Any
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevToolVault.Core.Commands;
using DevToolVault.Services; // Ou o namespace correto onde TreeGeneratorService está definido
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
                   //((RelayCommand)GenerateStructureCommand)?.RaiseCanExecuteChanged();
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
                    //((RelayCommand)GenerateStructureCommand)?.RaiseCanExecuteChanged();
                   // ((RelayCommand)BrowseFolderCommand)?.RaiseCanExecuteChanged();
                   //((RelayCommand)SaveStructureCommand)?.RaiseCanExecuteChanged();
                    //((RelayCommand)CopyStructureCommand)?.RaiseCanExecuteChanged();
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
        // CloseCommand pode ser útil, mas geralmente a View lida com o fechamento

        public event PropertyChangedEventHandler PropertyChanged;

        public EstruturaViewModel(FileFilterManager filterManager)
        {
            _filterManager = filterManager ?? throw new ArgumentNullException(nameof(filterManager));
            UpdateCurrentProfileName();

            BrowseFolderCommand = new RelayCommand(async () => await BrowseFolderAsync(), () => !IsGenerating);
            GenerateStructureCommand = new RelayCommand(async () => await GenerateStructureAsync(), () => !IsGenerating && !string.IsNullOrWhiteSpace(SelectedPath) && Directory.Exists(SelectedPath));
            SaveStructureCommand = new RelayCommand(SaveStructure, () => !IsGenerating && !string.IsNullOrWhiteSpace(StructureText));
            CopyStructureCommand = new RelayCommand(CopyStructure, () => !IsGenerating && !string.IsNullOrWhiteSpace(StructureText));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task BrowseFolderAsync()
        {
            var dialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrWhiteSpace(SelectedPath) && Directory.Exists(SelectedPath))
            {
                dialog.SelectedPath = SelectedPath;
            }

            // Obter a janela ativa para ser o Owner do diálogo
            var ownerWindow = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive) ?? System.Windows.Application.Current.MainWindow;

            if (dialog.ShowDialog(ownerWindow) == true)
            {
                SelectedPath = dialog.SelectedPath;
                //((RelayCommand)GenerateStructureCommand)?.RaiseCanExecuteChanged();
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
            UpdateCurrentProfileName(); // Atualiza o nome do perfil antes de gerar

            try
            {
                // Usar TreeGeneratorService (da versão nova) para gerar a árvore
                var treeGenerator = new TreeGeneratorService(_filterManager);
                var fileSystemItems = await Task.Run(() => treeGenerator.GenerateTree(SelectedPath));

                // Converter a lista de FileSystemItem em uma representação textual
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

            var ownerWindow = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive) ?? System.Windows.Application.Current.MainWindow;
            if (saveDialog.ShowDialog(ownerWindow) == true)
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, StructureText);
                    System.Windows.MessageBox.Show("Arquivo salvo com sucesso.", "Sucesso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erro ao salvar arquivo: {ex.Message}", "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                    System.Windows.MessageBox.Show("Estrutura copiada para a área de transferência.", "Sucesso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erro ao copiar para a área de transferência: {ex.Message}", "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void UpdateCurrentProfileName()
        {
            var activeProfile = _filterManager.GetActiveProfile();
            CurrentProfileName = activeProfile?.Name ?? "Nenhum";
        }

        // Se o FileFilterManager puder notificar sobre mudanças no perfil ativo,
        // o ViewModel poderia se inscrever nesse evento e chamar UpdateCurrentProfileName().
        // Por exemplo: _filterManager.ActiveProfileChanged += (s, e) => UpdateCurrentProfileName();
        // Para esta implementação, vamos assumir que o nome é atualizado ao gerar.
    }
}