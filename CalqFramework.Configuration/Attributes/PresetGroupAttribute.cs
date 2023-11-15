using System;
using System.Security.Cryptography.X509Certificates;

namespace CalqFramework.Configuration.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PresetGroupAttribute : Attribute {

        public string Name { get; }

        public PresetGroupAttribute(string name) {
            Name = name;
        }
    }
}
