// DevToolVault_Refatorado/Features/Export/ExportarCodigoWindow.xaml.cs
using System.Windows;
using DevToolVault.Refatorado.Core.Services;
using DevToolVault.Core.Models;

namespace DevToolVault.Features.Export
{
    public partial class ExportarCodigoWindow : Window
    {
        public ExportarCodigoWindow(ExportarCodigoViewModel viewModel) // ViewModel injetado
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // --- Manipuladores de Evento para botões que NÃO usam comandos do ViewModel ---
        // Estes interagem diretamente com o UserControl FileSystemTreeView

        private void BtnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            // Chama o método do UserControl - garantindo que fileTreeView existe
            fileTreeView?.SetAllExpanded(true); // Adicionado ? para segurança
        }

        private void BtnCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            // Chama o método do UserControl - garantindo que fileTreeView existe
            fileTreeView?.SetAllExpanded(false); // Adicionado ? para segurança
        }

        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            // Chama o método do UserControl para selecionar tudo - garantindo que fileTreeView existe
            fileTreeView?.SetAllItemsChecked(true); // Adicionado ? para segurança
        }

        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            // Chama o método do UserControl para desmarcar tudo - garantindo que fileTreeView existe
            fileTreeView?.SetAllItemsChecked(false); // Adicionado ? para segurança
        }

        // --- Fim dos manipuladores de evento ---
    }
}