using System.Collections.Generic;

namespace Captr
{
	public class CaptrOptions
	{
		/// <summary>
		/// List that contains a list of options as key/value pairs
		/// </summary>
		private readonly Dictionary<string, string> _configItems;

		public CaptrOptions()
		{
			_configItems = new(0);
		}

		public string GetConfigItem(string key)
		{
			_configItems.TryGetValue(key, out string configItem);
			return configItem;
		}

		public void SetConfigItem(string key, string value) => _configItems.Add(key, value);
	}
}
