using Calq.Configuration;
using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Calq.ConfigurationTest {

    public class ConfigTest {

        JsonSerializerOptions serializerOptions;
        string jsonSerializerResult;


        public ConfigTest() {
            serializerOptions = new JsonSerializerOptions {
                IncludeFields = true
            };

            var jsonPath = "config/Calq.ConfigurationTest.TestConfiguration.json";
            var jsonText = File.ReadAllText(jsonPath);
            var testConfiguration = JsonSerializer.Deserialize<TestConfiguration>(jsonText, serializerOptions);
            jsonSerializerResult = JsonSerializer.Serialize(testConfiguration, serializerOptions);
        }

        [Fact]
        public void Test1() {
            var testConfiguration = new TestConfiguration();
            Config.Load(ref testConfiguration);

            Assert.Equal(JsonSerializer.Serialize(testConfiguration, serializerOptions), jsonSerializerResult);
        }

        [Fact]
        public void Test2() {
            var testConfiguration = Config.Load<TestConfiguration>();

            Assert.Equal(JsonSerializer.Serialize(testConfiguration, serializerOptions), jsonSerializerResult);
        }

        [Fact]
        public void Test3() {
            var testConfigurationA = Config.Load<TestConfiguration>();
            var testConfigurationB = Config.Load<TestConfiguration>();

            Assert.True(Object.ReferenceEquals(testConfigurationA, testConfigurationB));
        }

        [Fact]
        public void Test4() {
            Assert.Throws<ArgumentException>(() => {
                TestConfiguration testConfiguration = null;
                Config.Load(ref testConfiguration);
            });
        }
    }
}
