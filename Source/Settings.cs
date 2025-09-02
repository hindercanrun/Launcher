using System.Collections.Generic;
using System.Globalization;

namespace Launcher
{
	internal struct Settings
	{
		internal Dictionary<string, string> Values { get; private set; }

		internal Settings(Dictionary<string, string> initialSettings)
		{
			Values = initialSettings ?? new Dictionary<string, string>();
		}

		internal void Set(Dictionary<string, string> newSettings)
		{
			Values = newSettings ?? new Dictionary<string, string>();
		}

		internal Dictionary<string, string> Get()
		{
			return Values;
		}

		internal bool GetBoolean(string key)
		{
			if (Values.TryGetValue(key, out string value))
			{
				return bool.TryParse(value, out bool result) && result;
			}
			return false;
		}

		internal decimal GetDecimal(string key)
		{
			if (Values.TryGetValue(key, out var value))
			{
				// Okay so apparently this can have issues on German machines.
				// Their PCs will put it as 0,11 instead of 0.11, so hopefully this workaround works
				if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
				{
					return result;
				}
			}
			return 0M;
		}

		internal string GetString(string key)
		{
			return Values.TryGetValue(key, out string value) ? value : "";
		}

		internal void SetBoolean(string key, bool value)
		{
			Values[key] = value.ToString();
		}

		internal void SetDecimal(string key, decimal value)
		{
			Values[key] = value.ToString(CultureInfo.InvariantCulture);
		}

		internal void SetString(string key, string value)
		{
			Values[key] = value ?? "";
		}
	}
}