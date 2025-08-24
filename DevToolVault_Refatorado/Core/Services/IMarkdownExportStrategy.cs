// Core/Services/IMarkdownExportStrategy.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using DevToolVault.Core.Models;

namespace DevToolVault.Core.Services
{
    public interface IMarkdownExportStrategy
    {
        Task ExportAsync(List<FileSystemItem> files, string outputPath);
    }
}