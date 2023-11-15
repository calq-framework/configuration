using System.Collections.Generic;

namespace CalqFramework.Configuration {
    public interface IConfigurationItem {
        string? PresetGroup { get; }

        string Preset { get; set; }
        IEnumerable<string> AvailablePresets { get; }
        object Item { get; }
        void Reload();
    }
}