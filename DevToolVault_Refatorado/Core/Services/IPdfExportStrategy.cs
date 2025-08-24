// Core/Services/IPdfExportStrategy.cs
using System.Collections.Generic;
using DevToolVault.Core.Models;
using System.Threading.Tasks;

namespace DevToolVault.Core.Services
{
    public interface IPdfExportStrategy
    {
        Task ExportAsync(List<FileSystemItem> files, string outputPath);
    }
}