// Core/Services/ITextExportStrategy.cs
using System.Collections.Generic;
using DevToolVault.Core.Models;
using System.Threading.Tasks;

namespace DevToolVault.Core.Services
{
    public interface ITextExportStrategy
    {
        Task ExportAsync(List<FileSystemItem> files, string outputPath);
    }
}