using System.Collections.Generic;

namespace Launcher
{
	public struct Settings
	{
		private Dictionary<string, string> settings;

		public Settings(Dictionary<string, string> initialSettings)
		{
			settings = initialSettings ?? new Dictionary<string, string>();
		}

		public void Set(Dictionary<string, string> newSettings)
		{
			settings = newSettings ?? new Dictionary<string, string>();
		}

		public Dictionary<string, string> Get()
		{
			return settings;
		}

		public bool GetBoolean(string key)
		{
			if (settings.TryGetValue(key, out string value))
			{
				return bool.TryParse(value, out bool result) && result;
			}
			return false;
		}

		public decimal GetDecimal(string key)
		{
			if (settings.TryGetValue(key, out string value))
			{
				if (decimal.TryParse(value, out decimal result))
				{
					return result;
				}
			}
			return 0M;
		}

		public string GetString(string key)
		{
			return settings.TryGetValue(key, out string value) ? value : "";
		}

		public void SetBoolean(string key, bool value)
		{
			settings[key] = value.ToString();
		}

		public void SetDecimal(string key, decimal value)
		{
			settings[key] = value.ToString();
		}

		public void SetString(string key, string value)
		{
			settings[key] = value ?? "";
		}
	}
}