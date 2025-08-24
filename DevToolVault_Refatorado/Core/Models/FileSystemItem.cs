using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DevToolVault.Core.Models
{
    public class FileSystemItem : INotifyPropertyChanged
    {
        private bool _isChecked = true;
        private bool _isExpanded;

        public string FullName { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public FileSystemItem Parent { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; } = new ObservableCollection<FileSystemItem>();

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                    Parent?.OnChildCheckedChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RelativePath
        {
            get
            {
                if (Parent == null) return Name ?? string.Empty;
                var parentPath = Parent.RelativePath;
                return string.IsNullOrEmpty(parentPath) ? Name : Path.Combine(parentPath, Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void OnChildCheckedChanged()
        {
            if (Children.Count == 0) return;

            int checkedCount = Children.Count(c => c.IsChecked == true);
            int uncheckedCount = Children.Count(c => c.IsChecked == false);

            if (checkedCount == Children.Count)
                _isChecked = true;
            else if (uncheckedCount == Children.Count)
                _isChecked = false;
            else
                _isChecked = true; // WPF CheckBox não aceita null para bool

            OnPropertyChanged(nameof(IsChecked));
            Parent?.OnChildCheckedChanged();
        }
    }
}
