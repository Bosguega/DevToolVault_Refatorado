using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace DevToolVault.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDirectory && isDirectory)
                return PackIconKind.Folder;

            if (value is string fullPath)
            {
                string ext = Path.GetExtension(fullPath).ToLower();
                return ext switch
                {
                    ".cs" => PackIconKind.LanguageCsharp,
                    ".dart" => PackIconKind.FileDocument,
                    ".json" => PackIconKind.FileDocument,
                    ".xml" => PackIconKind.Xml,
                    ".yml" or ".yaml" => PackIconKind.FileDocument,
                    ".md" => PackIconKind.LanguageMarkdown,
                    ".txt" => PackIconKind.TextBox,
                    ".html" => PackIconKind.LanguageHtml5,
                    ".css" => PackIconKind.LanguageCss3,
                    ".js" => PackIconKind.LanguageJavascript,
                    ".ts" => PackIconKind.LanguageTypescript,
                    ".xaml" => PackIconKind.Xml,
                    ".config" or ".ini" => PackIconKind.Settings,
                    ".sln" or ".csproj" => PackIconKind.MicrosoftVisualStudio,
                    _ => PackIconKind.FileDocumentOutline
                };
            }

            return PackIconKind.FileDocument;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
