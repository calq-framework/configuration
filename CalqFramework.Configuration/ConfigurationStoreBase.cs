using CalqFramework.Configuration.Attributes;
using CalqFramework.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Configuration {
    public abstract class ConfigurationStoreBase<TPreset> : IConfigurationStore where TPreset : new() {

        protected readonly Dictionary<Type, IConfigurationItem> configurationItems = new();
        protected IConfigurationItem presetConfiguration;

        public IEnumerable<string> AvailablePresetGroups {
            get => configurationItems.Values.Select(x => x.PresetGroup).Where(x => x != null)!;
        }

        public ConfigurationStoreBase() {
            presetConfiguration = CreateConfigurationItem<TPreset>(null);
        }

        public void Reload() {
            presetConfiguration.Reload();
            foreach (var item in configurationItems.Values) {
                item.Reload();
            }
        }

        public T Load<T>() where T : new() {
            if (configurationItems.ContainsKey(typeof(T))) {
                return (T)configurationItems[typeof(T)].Item;
            }

            return Reload<T>();
        }

        public T Reload<T>() where T : new() {
            if (configurationItems.ContainsKey(typeof(T))) {
                configurationItems[typeof(T)].Reload();
            } else {
                configurationItems[typeof(T)] = CreateConfigurationItem<T>(ResolvePresetGroupValue<T>());
            }
            return (T)configurationItems[typeof(T)].Item;
        }

        protected abstract IConfigurationItem CreateConfigurationItem<T>(string? presetName) where T : new();

        private string? ResolvePresetGroupValue<T>() {
            var presetGroup = typeof(T).GetCustomAttribute<PresetGroupAttribute>()?.Name;

            if (presetGroup != null) {
                return Reflection.GetFieldOrPropertyValue(presetConfiguration.Item, presetGroup)?.ToString()!;
            }

            return null;
        }

        public IEnumerable<string> GetAvailablePresets(string presetGroup) {
            // assume all configuration items of given preset group have the same available presets
            var configurationItem = configurationItems.Values.Where(x => x.PresetGroup == presetGroup).FirstOrDefault();
            if (configurationItem != null) {
                return configurationItem.AvailablePresets;
            } else {
                return Enumerable.Empty<string>();
            }    
        }
    }
}
