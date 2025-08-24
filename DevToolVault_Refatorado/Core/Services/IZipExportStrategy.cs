// Core/Services/IZipExportStrategy.cs
using System.Collections.Generic;
using DevToolVault.Core.Models;
using System.Threading.Tasks;

namespace DevToolVault.Core.Services
{
    public interface IZipExportStrategy
    {
        Task ExportAsync(List<FileSystemItem> files, string outputPath);
    }
}