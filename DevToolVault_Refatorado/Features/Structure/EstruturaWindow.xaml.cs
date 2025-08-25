// DevToolVault_Refatorado/Features/Structure/EstruturaWindow.xaml.cs
using System.Windows;
using DevToolVault.Controls;
using DevToolVault.Refatorado.Core.Services; // Certifique-se de que FileSystemTreeView está acessível

namespace DevToolVault.Features.Structure
{
    public partial class EstruturaWindow : Window
    {
        private readonly FileFilterManager _filterManager;

        public EstruturaWindow(FileFilterManager filterManager)
        {
            InitializeComponent();
            _filterManager = filterManager;

            // Carrega a raiz do projeto, se houver
            var activeProfile = _filterManager.GetActiveProfile();
            // Removido: fileTreeView.LoadDirectory(activeProfile.RootPath, _filterManager);
            // Esta lógica deve ser gerenciada pelo ViewModel ou pela própria TreeView
        }

        // --- Métodos de evento adicionados para corresponder ao XAML ---
        private void BtnUpdateStructure_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para atualizar/exibir a estrutura
            // Esta precisa ser implementada com base em como a estrutura é gerada/exibida
            // Exemplo (precisa de adaptação):
            // var activeProfile = _filterManager.GetActiveProfile();
            // if (activeProfile != null)
            // {
            //     // Regenerar estrutura e atualizar txtStructure.Text
            //     // fileTreeView.LoadDirectory(activeProfile.RootPath, _filterManager); // Se LoadDirectory existisse
            // }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // --- Fim dos métodos de evento adicionados ---

        // Removido: Métodos que chamam métodos inexistentes em FileSystemTreeView
        // private void BtnRefresh_Click(...) { ... }
        // private void BtnExpandAll_Click(...) { ... }
        // private void BtnCollapseAll_Click(...) { ... }
        // private void ChkSelectAll_Checked(...) { ... }
        // private void ChkSelectAll_Unchecked(...) { ... }
    }
}