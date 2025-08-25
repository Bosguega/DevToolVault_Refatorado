// Core/Services/ITreeGeneratorService.cs
using DevToolVault.Core.Models; // Para FileSystemItem
using DevToolVault.Refatorado.Core.Models;
using DevToolVault.Services;     // Para FilterProfile
using System.Collections.Generic;

namespace DevToolVault.Core.Services
{
    public interface ITreeGeneratorService
    {
        // string GenerateTree(string rootPath, TreeOptions options); // Remover ou atualizar
        List<FileSystemItem> BuildFileSystemTree(string rootPath, FilterProfile profile);
    }
}