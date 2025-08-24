// Filters/FilterProfile.cs
using System.Collections.Generic;

namespace DevToolVault.Filters
{
    public class FilterProfile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> IgnorePatterns { get; set; } = new();
        public List<string> CodeExtensions { get; set; } = new();
        public bool IgnoreEmptyFolders { get; set; } = true;
        public bool ShowFileSize { get; set; } = false;
        public bool ShowSystemFiles { get; set; } = false;
        public bool ShowOnlyCodeFiles { get; set; } = false;
        public bool IsBuiltIn { get; set; } = false;
    }
}