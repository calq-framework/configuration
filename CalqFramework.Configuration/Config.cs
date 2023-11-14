using CalqFramework.Configuration.Attributes;
using CalqFramework.Serialization;
using CalqFramework.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CalqFramework.Configuration {

    public class Config {

        private static readonly Dictionary<Type, object> instances = new();
        private static object? presetConfiguration = null;

        public static DirectoryInfo ConfigDir { get; }

        static Config() {
            var configPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{Process.GetCurrentProcess().ProcessName}";
            ConfigDir = Directory.CreateDirectory(configPath);
        }

        private static string GetJsonPath(Type type, string presetGroupPattern) {
            // TODO save resolved json path and throw error if different type tries to use the same json path
            return $"{ConfigDir.FullName}/{type.FullName}.{presetGroupPattern}.json";
        }

        private static string GetJsonPath(Type type) {
            return GetJsonPath(type, "*");
        }

        public static T LoadPresetConfiguration<T>() where T : notnull, new() {
            if (presetConfiguration != null) {
                return (T)presetConfiguration;
            }
            presetConfiguration = Load<T>();
            return (T)presetConfiguration;
        }

        public static T ReloadPresetConfiguration<T>() where T : notnull, new() {
            foreach (var obj in instances) {
                // TODO detect what was changed and reload only changed preset groups
                Reload(obj);
            }

            presetConfiguration = Reload<T>();
            return (T)presetConfiguration;
        }

        public static IEnumerable<string> GetPresetNames<T>() {
            var configPresets = Directory.GetFiles(ConfigDir.FullName, GetJsonPath(typeof(T)));
            var presetNames = configPresets.Select(configPreset => configPreset.Split('.')[^2]);
            return presetNames;
        }

        public static Dictionary<Type, IEnumerable<string>> GetPresetNames(string groupName) {
            var presetsByType = new Dictionary<Type, IEnumerable<string>>();
            foreach (var obj in instances) {
                var objGroupName = obj.GetType().GetCustomAttribute<PresetGroupAttribute>()?.GroupName;
                if (objGroupName != null && objGroupName == groupName) {
                    var configPresets = Directory.GetFiles(ConfigDir.FullName, GetJsonPath(obj.GetType()));
                    var presetNames = configPresets.Select(configPreset => configPreset.Split('.')[^2]);
                    presetsByType.Add(obj.GetType(), presetNames);
                }
            }
            return presetsByType;
        }

        public static T Load<T>() where T : notnull, new() {
            if (instances.ContainsKey(typeof(T))) {
                return (T)instances[typeof(T)];
            }

            return Reload<T>();
        }

        private static T Reload<T>() where T : notnull, new() {
            var instance = new T(); ;
            Load(instance);
            instances[typeof(T)] = instance;

            return instance;
        }

        private static void Reload(object obj) {
            Load(obj);
        }

        private static void Load<T>(T obj) where T : notnull {
            var presetName = "default";
            var objGroupName = obj.GetType().GetCustomAttribute<PresetGroupAttribute>()?.GroupName;
            if (objGroupName != null && presetConfiguration != null) {
                var loadedPresetName = Reflection.GetFieldOrPropertyValue(presetConfiguration, objGroupName)?.ToString();
                if (loadedPresetName != null) {
                    presetName = loadedPresetName;
                }
            }

            var jsonPath = GetJsonPath(typeof(T), presetName);
            var jsonText = File.ReadAllText(jsonPath);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonText);

            new JsonSerializer().Populate(jsonBytes, obj);
        }
    }
}
