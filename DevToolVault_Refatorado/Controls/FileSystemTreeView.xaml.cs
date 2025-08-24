using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevToolVault.Controls
{
    public partial class FileSystemTreeView : UserControl
    {
        public ObservableCollection<FileSystemItem> Items { get; set; } = new();

        public FileSystemTreeView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void LoadDirectory(string rootPath)
        {
            Items.Clear();
            if (Directory.Exists(rootPath))
            {
                Items.Add(CreateDirectoryNode(rootPath));
            }
        }

        private FileSystemItem CreateDirectoryNode(string path)
        {
            var dirItem = new FileSystemItem
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                IsDirectory = true
            };

            foreach (var dir in Directory.GetDirectories(path))
                dirItem.Children.Add(CreateDirectoryNode(dir));

            foreach (var file in Directory.GetFiles(path))
                dirItem.Children.Add(new FileSystemItem
                {
                    Name = Path.GetFileName(file),
                    FullPath = file,
                    IsDirectory = false
                });

            return dirItem;
        }

        /// <summary>
        /// Retorna os itens selecionados como lista.
        /// </summary>
        public List<FileSystemItem> GetSelectedItems()
        {
            var selected = new List<FileSystemItem>();
            foreach (var item in Items)
                CollectSelectedItems(item, selected);

            return selected;
        }

        private void CollectSelectedItems(FileSystemItem item, List<FileSystemItem> selected)
        {
            if (item.IsChecked == true && !item.IsDirectory)
                selected.Add(item);

            foreach (var child in item.Children)
                CollectSelectedItems(child, selected);
        }

        /// <summary>
        /// Marca/desmarca todos os itens.
        /// </summary>
        public void SetAllItemsChecked(bool isChecked)
        {
            foreach (var item in Items)
                SetItemCheckedRecursive(item, isChecked);
        }

        private void SetItemCheckedRecursive(FileSystemItem item, bool isChecked)
        {
            item.IsChecked = isChecked;
            foreach (var child in item.Children)
                SetItemCheckedRecursive(child, isChecked);
        }

        /// <summary>
        /// Expande/recolhe todos os itens.
        /// </summary>
        public void SetAllExpanded(bool isExpanded)
        {
            foreach (var item in Items)
                SetItemExpandedRecursive(item, isExpanded);
        }

        private void SetItemExpandedRecursive(FileSystemItem item, bool isExpanded)
        {
            item.IsExpanded = isExpanded;
            foreach (var child in item.Children)
                SetItemExpandedRecursive(child, isExpanded);
        }
    }

    public class FileSystemItem : DependencyObject
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsDirectory { get; set; }
        public bool IsExpanded { get; set; }
        public bool? IsChecked { get; set; } = false;
        public ObservableCollection<FileSystemItem> Children { get; set; } = new();
    }
}
