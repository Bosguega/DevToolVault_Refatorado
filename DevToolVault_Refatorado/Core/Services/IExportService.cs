// Core/Services/IExportService.cs
using System.Collections.Generic;
using DevToolVault.Core.Models;
using System.Threading.Tasks;

namespace DevToolVault.Core.Services
{
    public interface IExportService
    {
        Task ExportAsync(List<FileSystemItem> files, string outputPath, ExportFormat format);
    }

    public enum ExportFormat
    {
        Text,
        Markdown,
        Pdf,
        Zip
    }
}