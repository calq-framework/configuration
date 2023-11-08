using System;
using System.Security.Cryptography.X509Certificates;

namespace CalqFramework.Configuration.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PresetGroupAttribute : Attribute {

        public string GroupName { get; }

        public PresetGroupAttribute(string groupName) {
            GroupName = groupName;
        }
    }
}
