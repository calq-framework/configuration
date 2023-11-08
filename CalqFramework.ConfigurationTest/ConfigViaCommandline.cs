#pragma warning disable CS0649

using CalqFramework.Configuration.Attributes;

namespace CalqFramework.ConfigurationTest {
    [OptionsAttribute]
    class ConfigViaCommandline {
        public int port;
    }
}
