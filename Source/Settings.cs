using System.Collections.Generic;

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
		string value;
		if (settings.TryGetValue(key, out value))
		{
			bool result;
			return bool.TryParse(value, out result) && result;
		}
		return false;
	}

	public decimal GetDecimal(string key)
	{
		string value;
		if (settings.TryGetValue(key, out value))
		{
			decimal result;
			if (decimal.TryParse(value, out result))
				return result;
		}
		return 0M;
	}

	public string GetString(string key)
	{
		string value;
		return settings.TryGetValue(key, out value) ? value : "";
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