using CalqFramework.Configuration;
using System.IO;
using System.Text.Json;
using Xunit;

namespace CalqFramework.ConfigurationTest {


    [Collection("Sequential")]
    public class ConfigTest {

        static ConfigTest() {
            var configFolder = new DirectoryInfo("config");
            foreach (var configFile in Config.ConfigDir.GetFiles()) {
                configFile.Delete();
            }
            foreach (var configFile in configFolder.GetFiles()) {
                File.Copy($"{configFolder.FullName}/{configFile.Name}", $"{Config.ConfigDir.FullName}/{configFile.Name}");
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
            var testConfiguration = Config.Load<PlainConfiguration>();

            var serializedJson = Serialize<PlainConfiguration>($"config/CalqFramework.ConfigurationTest.{nameof(PlainConfiguration)}.default.json");
            Assert.Equal(Serialize(testConfiguration), serializedJson);
        }

        [Fact]
        public void Test3() {
            var configA = Config.Load<PlainConfiguration>();
            var configB = Config.Load<PlainConfiguration>();

            Assert.True(ReferenceEquals(configA, configB));
        }

        //[Fact]
        //public void Test5() {
        //    var config = Config.Load<ConfigViaCommandline>();

        //    Assert.NotEqual(0, config.port);
        //}

        [Fact]
        public void Test6() {
            _ = Config.LoadPresetConfiguration<PresetConfig>();
            var config = Config.Load<ConfigViaPresetGroup>();

            var serializedJson = Serialize<ConfigViaPresetGroup>($"config/CalqFramework.ConfigurationTest.{nameof(ConfigViaPresetGroup)}.test.json");
            Assert.Equal(Serialize(config), serializedJson);
        }

    }
}
