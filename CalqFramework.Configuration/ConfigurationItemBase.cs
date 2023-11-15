using CalqFramework.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Configuration {
    public abstract class ConfigurationItemBase<TItem> : IConfigurationItem where TItem : new() {
        private string preset;
        public string? PresetGroup { get; }
        public object Item{ get; }
        public string Preset {
            get => preset ;
            set {
                if (preset != value) {
                    Reload(value);
                    preset = value;
                }
            }
        }

        public abstract IEnumerable<string> AvailablePresets { get; }

        public ConfigurationItemBase(string preset) {
            PresetGroup = typeof(TItem).GetCustomAttribute<PresetGroupAttribute>()?.Name;
            Item = new TItem();
            this.preset = preset;
        }

        public abstract void Reload(string preset);

        public void Reload() {
            Reload(Preset);
        }
    }
}