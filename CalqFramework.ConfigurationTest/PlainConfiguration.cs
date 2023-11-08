#pragma warning disable CS0649

namespace CalqFramework.ConfigurationTest {
    class PlainConfiguration {
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
