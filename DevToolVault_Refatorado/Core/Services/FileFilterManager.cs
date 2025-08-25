using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using DevToolVault.Refatorado.Core.Models;

namespace DevToolVault.Refatorado.Core.Services
{
    public class FileFilterManager
    {
        private readonly string _filtersDirectory;
        private List<FilterProfile> _profiles = new();
        private FilterProfile _activeProfile;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public FileFilterManager(string filtersDirectory = null)
        {
            _filtersDirectory = filtersDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DevToolVault", "Filters");
            EnsureDirectoryExists();
            LoadDefaultFilters();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_filtersDirectory))
                Directory.CreateDirectory(_filtersDirectory);
        }

        private void LoadDefaultFilters()
        {
            var builtInProfiles = new[]
            {
                new
                {
                    Name = "Flutter",
                    Description = "Filtro para projetos Flutter",
                    IgnorePatterns = new List<string>
                    {
                        "android", "ios", "linux", "macos", "windows", "web",
                        "build", "dart_tool", ".dart_tool", ".pub", ".flutter-plugins",
                        ".idea", ".vscode", ".vs",
                        "*.g.dart", "*.r.dart", "*.gr.dart", "*.freezed.dart",
                        "*.inject.dart", "*.mocks.dart",
                        "*.apk", "*.aab", "*.ipa", "*.app", "*.exe", "*.dll",
                        "*.so", "*.dylib", "*.jar", "*.aar", "*.framework",
                        ".DS_Store", "Thumbs.db", "desktop.ini",
                        "*.log", "*.cache", "*.tmp"
                    },
                    CodeExtensions = new List<string> { ".dart", ".yaml", ".yml", ".json", ".xml", ".html", ".css", ".js", ".ts" }
                },
                // ... outros perfis ...
            };

            foreach (var p in builtInProfiles)
            {
                _profiles.Add(new FilterProfile
                {
                    Name = p.Name,
                    Description = p.Description,
                    IgnorePatterns = p.IgnorePatterns,
                    CodeExtensions = p.CodeExtensions,
                    IgnoreEmptyFolders = true,
                    ShowFileSize = false,
                    ShowSystemFiles = false,
                    ShowOnlyCodeFiles = true,
                    IsBuiltIn = true
                });
            }

            _activeProfile = _profiles.FirstOrDefault();
            LoadCustomProfiles();
        }

        private void LoadCustomProfiles()
        {
            try
            {
                var filterFiles = Directory.GetFiles(_filtersDirectory, "*.json");
                foreach (var file in filterFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var profile = JsonSerializer.Deserialize<FilterProfile>(json);
                        if (profile != null)
                        {
                            profile.IsBuiltIn = false;
                            _profiles.Add(profile);
                        }
                    }
                    catch
                    {
                        // Ignora arquivos inválidos
                    }
                }
            }
            catch
            {
                // Ignora erros ao carregar perfis
            }
        }

        public IEnumerable<FilterProfile> GetProfiles() => _profiles;
        public FilterProfile GetActiveProfile() => _activeProfile;

        public void SetActiveProfile(FilterProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            _activeProfile = profile;
        }

        public void SaveProfile(FilterProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            var fileName = $"{profile.Name}.json";
            var filePath = Path.Combine(_filtersDirectory, fileName);
            var json = JsonSerializer.Serialize(profile, _jsonOptions);
            File.WriteAllText(filePath, json);

            var existing = _profiles.FirstOrDefault(p => p.Name == profile.Name);
            if (existing != null)
                _profiles.Remove(existing);
            _profiles.Add(profile);
        }

        public void DeleteProfile(FilterProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            if (profile.IsBuiltIn) return;

            var fileName = $"{profile.Name}.json";
            var filePath = Path.Combine(_filtersDirectory, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            _profiles.Remove(profile);
            if (_activeProfile == profile)
                _activeProfile = _profiles.FirstOrDefault();
        }

        public FilterProfile CreateProfile(string name, string description = null)
        {
            var profile = new FilterProfile
            {
                Name = name,
                Description = description,
                IgnorePatterns = new(_activeProfile.IgnorePatterns),
                CodeExtensions = new(_activeProfile.CodeExtensions),
                IgnoreEmptyFolders = _activeProfile.IgnoreEmptyFolders,
                ShowFileSize = _activeProfile.ShowFileSize,
                ShowSystemFiles = _activeProfile.ShowSystemFiles,
                ShowOnlyCodeFiles = _activeProfile.ShowOnlyCodeFiles,
                IsBuiltIn = false
            };
            return profile;
        }
    }
}