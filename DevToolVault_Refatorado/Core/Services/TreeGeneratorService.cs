// Core/Services/TreeGeneratorService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevToolVault.Core.Models;
using DevToolVault.Filters;

namespace DevToolVault.Core.Services
{
    public class TreeGeneratorService : ITreeGeneratorService
    {
        private readonly FileStatistics _statistics;

        public TreeGeneratorService(FileStatistics statistics)
        {
            _statistics = statistics;
        }

        public string GenerateTree(string rootPath, TreeOptions options)
        {
            var builder = new StringBuilder();
            var rootDir = new DirectoryInfo(rootPath);
            builder.AppendLine(rootDir.FullName);
            AppendDirectoryContents(rootDir, builder, "", options);
            return builder.ToString();
        }

        private void AppendDirectoryContents(DirectoryInfo dir, StringBuilder sb, string indent, TreeOptions options)
        {
            _statistics.TotalFolders++;
            try
            {
                FileInfo[] files;
                DirectoryInfo[] subDirs;
                try
                {
                    files = dir.GetFiles();
                    subDirs = dir.GetDirectories();
                }
                catch (UnauthorizedAccessException)
                {
                    sb.AppendLine($"{indent}\\-- | Acesso negado");
                    return;
                }

                var fileFilter = new FileFilter(options);
                var visibleFiles = files.Where(f => !fileFilter.ShouldIgnore(f.FullName, false)).ToArray();
                var visibleSubDirs = subDirs.Where(d => !fileFilter.ShouldIgnore(d.FullName, true)).ToArray();

                int totalItems = visibleFiles.Length + visibleSubDirs.Length;
                if (totalItems == 0 && options.IgnoreEmptyFolders)
                    return;

                for (int i = 0; i < visibleFiles.Length; i++)
                {
                    var file = visibleFiles[i];
                    bool isLast = (i == visibleFiles.Length - 1) && (visibleSubDirs.Length == 0);
                    string symbol = isLast ? "\\-- " : "|-- ";
                    string size = options.ShowFileSize ? $" ({_statistics.FormatFileSize(file.Length)})" : "";
                    sb.AppendLine($"{indent}{symbol}{file.Name}{size}");
                    _statistics.TotalFiles++;
                    _statistics.TotalSize += file.Length;
                }

                for (int i = 0; i < visibleSubDirs.Length; i++)
                {
                    var subDir = visibleSubDirs[i];
                    bool isLast = (i == visibleSubDirs.Length - 1); // && (visibleFiles.Length == 0); // Removido para alinhar com a lógica padrão
                    string symbol = isLast ? "\\-- " : "|-- ";
                    string newIndent = indent + (isLast ? "    " : "|   ");
                    sb.AppendLine($"{indent}{symbol}{subDir.Name}/");
                    AppendDirectoryContents(subDir, sb, newIndent, options);
                }

                if (totalItems == 0)
                {
                    sb.AppendLine($"{indent}\\-- | (vazio)");
                }
            }
            // Corrigido CS0168: Removido nome da variável não usada ou usado conforme necessário
            catch (Exception ex) // Se for para logar o erro, use 'ex'
            {
                sb.AppendLine($"{indent}\\-- | Erro ao ler: {ex.Message}"); // Exemplo de uso
            }
        }

        public List<FileSystemItem> BuildFileSystemTree(string rootPath, FilterProfile profile)
        {
            var rootItem = new FileSystemItem
            {
                FullName = rootPath,
                Name = Path.GetFileName(rootPath) ?? rootPath,
                IsDirectory = true,
                IsChecked = true
            };

            var options = TreeOptions.FromFilterProfile(profile);
            var fileFilter = new FileFilter(options);
            PopulateDirectory(rootItem, fileFilter);
            return new List<FileSystemItem> { rootItem };
        }

        private void PopulateDirectory(FileSystemItem parentItem, FileFilter fileFilter)
        {
            try
            {
                var dirInfo = new DirectoryInfo(parentItem.FullName);
                foreach (var dir in dirInfo.GetDirectories())
                {
                    if (!fileFilter.ShouldIgnore(dir.FullName, true))
                    {
                        var dirItem = new FileSystemItem
                        {
                            FullName = dir.FullName,
                            Name = dir.Name,
                            IsDirectory = true,
                            IsChecked = true,
                            Parent = parentItem
                        };
                        parentItem.Children.Add(dirItem);
                        PopulateDirectory(dirItem, fileFilter);
                    }
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    if (!fileFilter.ShouldIgnore(file.FullName, false))
                    {
                        var fileItem = new FileSystemItem
                        {
                            FullName = file.FullName,
                            Name = file.Name,
                            IsDirectory = false,
                            IsChecked = true,
                            Parent = parentItem
                        };
                        parentItem.Children.Add(fileItem);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignora diretórios sem acesso
            }
            // Corrigido CS0168: Removido nome da variável não usada
            catch (Exception) // Se não for usar 'ex', remova o nome
            {
                // Logar erro se necessário
            }
        }
    }
}