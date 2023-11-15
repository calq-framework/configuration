using CalqFramework.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CalqFramework.Configuration {
    public class JsonConfigurationItem<TItem> : ConfigurationItemBase<TItem> where TItem : new() {

        public DirectoryInfo ConfigDir { get; }

        public override IEnumerable<string> AvailablePresets {
            get {
                var presetFiles = Directory.GetFiles(ConfigDir.FullName, GetJsonPath("*"));
                var presets = presetFiles.Select(presetFile => presetFile.Split('.')[^2]);
                return presets;
            }
        }

        public JsonConfigurationItem(string preset, DirectoryInfo configDir) : base(preset) {
            ConfigDir = configDir;
            Reload();
        }

        private string GetJsonPath(string preset) {
            // TODO resolve path and throw error if newly created item resolved to already resolved path
            return $"{ConfigDir?.FullName}/{typeof(TItem).FullName}.{preset}.json";
        }

        public override void Reload(string preset) {
            var jsonPath = GetJsonPath(preset);

            if (!Path.Exists(jsonPath)) {
                return;
            }

            var jsonText = File.ReadAllText(jsonPath);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonText);

            new JsonSerializer().Populate(jsonBytes, Item);
        }
    }
}