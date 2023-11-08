#pragma warning disable CS0649

using CalqFramework.Configuration.Attributes;

namespace CalqFramework.ConfigurationTest {

    [PresetGroup(nameof(PresetConfig.TestGroup))]
    class ConfigViaPresetGroup {
        public class Inner {
            public int a;
            public int b;
        }

        public int integer;
        public bool boolean;
        public Inner inner;
        public string text;
        public string nullText = "text";
        public int port;
    }
}
