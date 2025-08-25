// DevToolVault_Refatorado/ViewModels/ExportarCodigoViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using DevToolVault.Core.Models;
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Refatorado.Core.Services;
using DevToolVault.Services; // Corrigido: Se TreeGeneratorService estiver aqui

namespace DevToolVault.ViewModels
{
    public class ExportarCodigoViewModel : INotifyPropertyChanged
    {
        private readonly FileFilterManager _filterManager;
        private readonly TreeGeneratorService _treeGenerator; // Corrigido: TreeGeneratorService
        private List<FileSystemItem> _fileSystemItems;
        private string _currentPath;
        private bool _isLoading;
        private FilterProfile _activeProfile;

        // Corrigido: FileFilterManager
        public ExportarCodigoViewModel(FileFilterManager filterManager)
        {
            _filterManager = filterManager ?? throw new ArgumentNullException(nameof(filterManager));
            // Corrigido: TreeGeneratorService
            _treeGenerator = new TreeGeneratorService(filterManager);
            _activeProfile = _filterManager.GetActiveProfile();
        }

        public List<FileSystemItem> FileSystemItems
        {
            get => _fileSystemItems;
            private set
            {
                _fileSystemItems = value;
                OnPropertyChanged(nameof(FileSystemItems));
            }
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public FilterProfile ActiveProfile
        {
            get => _activeProfile;
            set
            {
                _activeProfile = value;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public IEnumerable<FilterProfile> AvailableProfiles => _filterManager.GetProfiles();

        public async Task LoadDirectoryAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Diretório inválido ou inexistente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsLoading = true;
            CurrentPath = path;

            try
            {
                // Corrigido: Usar FullPath em vez de FullName se for o caso
                FileSystemItems = await Task.Run(() => _treeGenerator.GenerateTree(path));
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Acesso negado ao diretório: {ex.Message}", "Erro de Permissão", MessageBoxButton.OK, MessageBoxImage.Error);
                FileSystemItems = new List<FileSystemItem>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar diretório: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                FileSystemItems = new List<FileSystemItem>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SetActiveProfile(FilterProfile profile)
        {
            if (profile == null) return;

            _filterManager.SetActiveProfile(profile);
            ActiveProfile = profile;

            // Recarrega o diretório atual com o novo perfil
            if (!string.IsNullOrEmpty(CurrentPath))
            {
                await LoadDirectoryAsync(CurrentPath);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}