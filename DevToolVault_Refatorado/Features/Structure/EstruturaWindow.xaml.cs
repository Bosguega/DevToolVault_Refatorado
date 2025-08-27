// DevToolVault_Refatorado/Features/Structure/EstruturaWindow.xaml.cs
using System.Windows;
using DevToolVault.Refatorado.Core.Services; // Se ainda for usado para algo específico no code-behind
using DevToolVault.Features.Structure; // Para o tipo do ViewModel
using Microsoft.Extensions.DependencyInjection; // Para IServiceProvider
using System;
using System.Windows.Controls;
using DevToolVault.Converters;

namespace DevToolVault.Features.Structure
{
    public partial class EstruturaWindow : Window
    {
        // Construtor atualizado para receber o ViewModel injetado
        public EstruturaWindow(EstruturaViewModel viewModel) // ViewModel injetado
        {
            InitializeComponent();
            // Define o DataContext para o ViewModel injetado
            DataContext = viewModel;
        }
    }
}