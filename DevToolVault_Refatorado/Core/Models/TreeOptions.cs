// DevToolVault_Refatorado/Core/Models/TreeOptions.cs
using System.Collections.Generic;

namespace DevToolVault.Core.Models
{
    public class TreeOptions
    {
        // Propriedades existentes (poderiam ser redundantes se FilterProfile for usado)
        public bool IgnoreEmptyDirectories { get; set; }
        public bool IgnoreHiddenFiles { get; set; }
        public List<string> IgnoredExtensions { get; set; } = new();
        public List<string> IgnoredNames { get; set; } = new();

        // --- Propriedades adicionadas para compatibilidade com FileFilter ---
        public bool IgnoreEmptyFolders { get; set; } // Adicionado
        public bool ShowOnlyCodeFiles { get; set; }  // Adicionado
        public bool ShowSystemFiles { get; set; } = true; // Adicionado (padrão true)
        public List<string> IgnorePatterns { get; set; } = new(); // Adicionado
        public List<string> CodeExtensions { get; set; } = new(); // Adicionado
        // --- Fim das propriedades adicionadas ---
    }
}