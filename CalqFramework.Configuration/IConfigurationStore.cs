using System;
using System.Collections.Generic;

namespace CalqFramework.Configuration {
    public interface IConfigurationStore {

        public IEnumerable<string> AvailablePresetGroups { get; }

        public IEnumerable<string> GetAvailablePresets(string presetGroup);

        public T Load<T>() where T : new();

        public T Reload<T>() where T : new();

        public void Reload();
    }
}
