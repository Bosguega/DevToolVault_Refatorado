// DevToolVault_Refatorado/Features/Filters/FilterManagerWindow.xaml.cs
using DevToolVault.Core.Models; // <<< Corrigido o namespace para o FilterProfile
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Refatorado.Core.Services; // <<< Namespace correto para FileFilterManager
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevToolVault.Features.Filters
{
    /// <summary>
    /// Lógica interna para FilterManagerWindow.xaml
    /// </summary>
    public partial class FilterManagerWindow : Window
    {
        private readonly FileFilterManager _filterManager;

        public FilterManagerWindow(FileFilterManager filterManager)
        {
            InitializeComponent();
            _filterManager = filterManager ?? throw new ArgumentNullException(nameof(filterManager));
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            lstProfiles.ItemsSource = null; // Limpa a seleção/bindings antigos
            lstProfiles.ItemsSource = _filterManager.GetProfiles();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var newProfile = new FilterProfile
            {
                Name = "Novo Perfil",
                Description = "Descrição do novo perfil",
                IgnorePatterns = new System.Collections.Generic.List<string>(), // Inicializa listas vazias
                CodeExtensions = new System.Collections.Generic.List<string>(),
                IgnoreEmptyFolders = true,
                ShowFileSize = false,
                ShowSystemFiles = false,
                ShowOnlyCodeFiles = false,
                IsBuiltIn = false // Perfis novos não são embutidos
            };

            var editWindow = new FilterEditWindow(newProfile, false); // false = novo perfil
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    // Salva o novo perfil usando o FileFilterManager
                    _filterManager.SaveProfile(newProfile);
                    LoadProfiles(); // Recarrega a lista para mostrar o novo perfil

                    // Opcional: definir o novo perfil como ativo?
                    // _filterManager.SetActiveProfile(newProfile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao criar o perfil: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstProfiles.SelectedItem is FilterProfile selectedProfile)
            {
                if (selectedProfile.IsBuiltIn)
                {
                    MessageBox.Show("Perfis embutidos não podem ser editados.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Cria uma cópia para edição, para não modificar o original diretamente
                var profileToEdit = new FilterProfile
                {
                    Name = selectedProfile.Name,
                    Description = selectedProfile.Description,
                    IgnorePatterns = new System.Collections.Generic.List<string>(selectedProfile.IgnorePatterns),
                    CodeExtensions = new System.Collections.Generic.List<string>(selectedProfile.CodeExtensions),
                    IgnoreEmptyFolders = selectedProfile.IgnoreEmptyFolders,
                    ShowFileSize = selectedProfile.ShowFileSize,
                    ShowSystemFiles = selectedProfile.ShowSystemFiles,
                    ShowOnlyCodeFiles = selectedProfile.ShowOnlyCodeFiles,
                    IsBuiltIn = selectedProfile.IsBuiltIn
                    // RootPath não é editável aqui, assumindo que não é parte do filtro em si
                };

                var editWindow = new FilterEditWindow(profileToEdit, true); // true = editando
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        // Atualiza as propriedades do perfil original com os valores editados
                        selectedProfile.Name = profileToEdit.Name;
                        selectedProfile.Description = profileToEdit.Description;
                        selectedProfile.IgnorePatterns = new System.Collections.Generic.List<string>(profileToEdit.IgnorePatterns);
                        selectedProfile.CodeExtensions = new System.Collections.Generic.List<string>(profileToEdit.CodeExtensions);
                        selectedProfile.IgnoreEmptyFolders = profileToEdit.IgnoreEmptyFolders;
                        selectedProfile.ShowFileSize = profileToEdit.ShowFileSize;
                        selectedProfile.ShowSystemFiles = profileToEdit.ShowSystemFiles;
                        selectedProfile.ShowOnlyCodeFiles = profileToEdit.ShowOnlyCodeFiles;
                        // selectedProfile.IsBuiltIn e RootPath não mudam

                        // Salva as alterações
                        _filterManager.SaveProfile(selectedProfile);
                        LoadProfiles(); // Recarrega para atualizar a exibição
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao salvar o perfil: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um perfil para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstProfiles.SelectedItem is FilterProfile selectedProfile)
            {
                if (selectedProfile.IsBuiltIn)
                {
                    MessageBox.Show("Perfis embutidos não podem ser excluídos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"Tem certeza que deseja excluir o perfil '{selectedProfile.Name}'?",
                                             "Confirmar Exclusão",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _filterManager.DeleteProfile(selectedProfile);
                        LoadProfiles(); // Recarrega a lista após a exclusão
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir o perfil: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um perfil para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSetActive_Click(object sender, RoutedEventArgs e)
        {
            if (lstProfiles.SelectedItem is FilterProfile selectedProfile)
            {
                try
                {
                    _filterManager.SetActiveProfile(selectedProfile);
                    // Não precisa recarregar a lista, mas pode querer fechar a janela ou dar feedback
                    MessageBox.Show($"Perfil '{selectedProfile.Name}' definido como ativo.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao definir o perfil como ativo: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um perfil para definir como ativo.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Manipulador para o botão "Dupl..." (presumivelmente "Duplicar")
        // Adicionado com base na imagem do código da versão antiga, embora não esteja no XAML exportado.
        // Se o botão existir, o nome do método deve corresponder ao Click="..." no XAML.
        // private void BtnDuplicate_Click(object sender, RoutedEventArgs e)
        // {
        //     if (lstProfiles.SelectedItem is FilterProfile selectedProfile)
        //     {
        //         var duplicatedProfile = new FilterProfile
        //         {
        //             Name = $"{selectedProfile.Name} - Cópia",
        //             Description = selectedProfile.Description,
        //             IgnorePatterns = new System.Collections.Generic.List<string>(selectedProfile.IgnorePatterns),
        //             CodeExtensions = new System.Collections.Generic.List<string>(selectedProfile.CodeExtensions),
        //             IgnoreEmptyFolders = selectedProfile.IgnoreEmptyFolders,
        //             ShowFileSize = selectedProfile.ShowFileSize,
        //             ShowSystemFiles = selectedProfile.ShowSystemFiles,
        //             ShowOnlyCodeFiles = selectedProfile.ShowOnlyCodeFiles,
        //             IsBuiltIn = false // Perfis duplicados não são embutidos
        //         };
        //
        //         var editWindow = new FilterEditWindow(duplicatedProfile, false); // false = novo perfil
        //         editWindow.Owner = this;
        //         if (editWindow.ShowDialog() == true)
        //         {
        //             try
        //             {
        //                 _filterManager.SaveProfile(duplicatedProfile);
        //                 LoadProfiles();
        //             }
        //             catch (Exception ex)
        //             {
        //                 MessageBox.Show($"Erro ao duplicar o perfil: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        //             }
        //         }
        //     }
        //     else
        //     {
        //         MessageBox.Show("Selecione um perfil para duplicar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
        //     }
        // }
    }
}