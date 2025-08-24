// Core/Services/ITreeGeneratorService.cs
using System.Collections.Generic;
using DevToolVault.Core.Models;
using DevToolVault.Filters;

namespace DevToolVault.Core.Services
{
    public interface ITreeGeneratorService
    {
        string GenerateTree(string rootPath, TreeOptions options);
        List<FileSystemItem> BuildFileSystemTree(string rootPath, FilterProfile profile);
    }
}