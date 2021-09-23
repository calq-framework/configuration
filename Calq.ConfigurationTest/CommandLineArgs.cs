#pragma warning disable CS0649

using Calq.Configuration.Attributes;

namespace Calq.ConfigurationTest {
    [OptionsAttribute]
    class CommandLineArgs {
        public int port;
    }
}
