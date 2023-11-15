using CalqFramework.Configuration;
using System.IO;
using System.Text.Json;
using Xunit;

namespace CalqFramework.ConfigurationTest {


    [Collection("Sequential")]
    public class JsonConfigurationStoreTest {

        static JsonConfigurationStoreTest() {
            var configStore = new JsonConfigurationStore();
            var testConfigDir = new DirectoryInfo("config");
            foreach (var configFile in configStore.ConfigDir.GetFiles()) {
                configFile.Delete();
            }
            foreach (var configFile in testConfigDir.GetFiles()) {
                File.Copy($"{testConfigDir.FullName}/{configFile.Name}", $"{configStore.ConfigDir.FullName}/{configFile.Name}");
            }
        }

        public string Serialize<T>(string configFilePath) {
            var serializerOptions = new JsonSerializerOptions {
                IncludeFields = true
            };

            var jsonPath = configFilePath;
            var jsonText = File.ReadAllText(jsonPath);
            var testConfiguration = JsonSerializer.Deserialize<T>(jsonText, serializerOptions);
            var jsonSerializerResult = JsonSerializer.Serialize(testConfiguration, serializerOptions);
            return jsonSerializerResult;
        }

        public string Serialize<T>(T testConfiguration) {
            var serializerOptions = new JsonSerializerOptions {
                IncludeFields = true
            };

            return JsonSerializer.Serialize(testConfiguration, serializerOptions);
        }

        [Fact]
        public void Test2() {
            var testConfiguration = new JsonConfigurationStore().Load<SomeConfiguration>();

            var serializedJson = Serialize<SomeConfiguration>($"config/CalqFramework.ConfigurationTest.{nameof(SomeConfiguration)}.default.json");
            Assert.Equal(Serialize(testConfiguration), serializedJson);
        }

        [Fact]
        public void Test3() {
            var configStore = new JsonConfigurationStore();
            var configA = configStore.Load<SomeConfiguration>();
            var configB = configStore.Load<SomeConfiguration>();

            Assert.True(ReferenceEquals(configA, configB));
        }

        //[Fact]
        //public void Test5() {
        //    var config = new JsonConfigurationStore().Load<ConfigViaCommandline>();

        //    Assert.NotEqual(0, config.port);
        //}

        [Fact]
        public void Test6() {
            var configStore = new JsonConfigurationStore<PresetConfig>();
            var config = configStore.Load<ConfigViaPresetGroup>();

            var serializedJson = Serialize<ConfigViaPresetGroup>($"config/CalqFramework.ConfigurationTest.{nameof(ConfigViaPresetGroup)}.test.json");
            Assert.Equal(Serialize(config), serializedJson);
        }

    }
}
