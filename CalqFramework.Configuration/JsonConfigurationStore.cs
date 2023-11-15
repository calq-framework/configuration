using System;
using System.Diagnostics;
using System.IO;

namespace CalqFramework.Configuration {

    public class EmptyPresetConfiguration { }

    public class JsonConfigurationStore : JsonConfigurationStore<EmptyPresetConfiguration> { }

    public class JsonConfigurationStore<TPreset> : ConfigurationStoreBase<TPreset> where TPreset : new() {

        public DirectoryInfo ConfigDir { get; }

        public JsonConfigurationStore() {
            ConfigDir = GetConfigDir();
        }

        private DirectoryInfo GetConfigDir() {
            var configPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{Process.GetCurrentProcess().ProcessName}";
            return Directory.CreateDirectory(configPath);
        }

        protected override IConfigurationItem CreateConfigurationItem<T>(string? preset) {
            if (preset == null) {
                preset = "default";
            }

            // during store construction, preset configuration item is being created and ConfigDir isn't assigned yet
            return new JsonConfigurationItem<T>(preset, ConfigDir ?? GetConfigDir());
        }
    }
}
