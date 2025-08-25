// DevToolVault_Refatorado/Core/Models/FileFilter.cs
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevToolVault.Filters; // Corrigido: Se estiver em Filters/

namespace DevToolVault.Core.Models // Ou DevToolVault.Filters se for o local correto
{
    public class FileFilter
    {
        private readonly TreeOptions _options;

        public FileFilter(TreeOptions options)
        {
            _options = options;
        }

        public bool ShouldIgnore(string path, bool isDirectory)
        {
            var name = Path.GetFileName(path);
            var extension = Path.GetExtension(path).ToLowerInvariant(); // Corrigido: ToLowerInvariant

            if (isDirectory)
            {
                if (_options.IgnoreEmptyFolders && IsDirectoryEmpty(path))
                    return true;

                foreach (var pattern in _options.IgnorePatterns)
                {
                    // Corrigido: MatchesPattern
                    if (MatchesPattern(name, path, pattern)) return true;
                }
            }
            else
            {
                if (_options.ShowOnlyCodeFiles && _options.CodeExtensions != null)
                {
                    if (!_options.CodeExtensions.Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }

                foreach (var pattern in _options.IgnorePatterns) // Corrigido: IgnorePatterns
                {
                    // Corrigido: MatchesPattern
                    if (MatchesPattern(name, path, pattern)) return true;
                }
            }

            if (!_options.ShowSystemFiles)
            {
                try
                {
                    var attr = File.GetAttributes(path);
                    if ((attr & FileAttributes.Hidden) != 0 ||
                        (attr & FileAttributes.System) != 0 ||
                        (attr & FileAttributes.Temporary) != 0 ||
                        (attr & FileAttributes.Offline) != 0)
                    {
                        return true;
                    }
                }
                catch
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDirectoryEmpty(string path)
        {
            try
            {
                return !Directory.EnumerateFileSystemEntries(path).Any();
            }
            catch
            {
                return false;
            }
        }

        private bool MatchesPattern(string name, string fullPath, string pattern)
        {
            var normPath = fullPath.Replace('\\', '/').ToLowerInvariant();
            var normName = name.ToLowerInvariant();
            var normPattern = pattern.Replace('\\', '/').ToLowerInvariant(); // Corrigido: ToLowerInvariant

            if (normPattern.StartsWith("*."))
            {
                var ext = normPattern.Substring(1);
                return normName.EndsWith(ext, StringComparison.OrdinalIgnoreCase);
            }
            else if (normPattern.Contains("*") || normPattern.Contains("?")) // Corrigido: *
            {
                var patternHasSlash = normPattern.Contains("/"); // Corrigido: /
                var target = patternHasSlash ? normPath : normName;
                var regexPattern = "^" + Regex.Escape(normPattern)
                    .Replace("\\*", ".*") // Corrigido: *
                    .Replace("\\?", ".") + "$"; // Corrigido: ?
                return Regex.IsMatch(target, regexPattern, RegexOptions.IgnoreCase);
            }
            else
            {
                var patternHasSlash = normPattern.Contains("/"); // Corrigido: /
                return patternHasSlash
                    ? normPath.Contains(normPattern)
                    : string.Equals(normName, normPattern, StringComparison.OrdinalIgnoreCase); // Corrigido: OrdinalIgnoreCase
            }
        }

        public bool IsCodeFile(string path)
        {
            if (_options.CodeExtensions == null || string.IsNullOrEmpty(path)) return false; // Corrigido: false
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return _options.CodeExtensions.Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}