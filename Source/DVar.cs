namespace Launcher
{
	internal readonly struct DVar
	{
		internal string Name { get; }
		internal string Description { get; }
		internal bool IsDecimal { get; }
		internal decimal DecimalMin { get; }
		internal decimal DecimalMax { get; }
		internal decimal DecimalIncrement { get; }

		// DVar: String
		internal DVar(
			string name,
			string description)
		{
			Name = name;
			Description = description;
			IsDecimal = false;
			DecimalMin = 0M;
			DecimalMax = 0M;
			DecimalIncrement = 0M;
		}

		// DVar: Decimal
		internal DVar(
			string name,
			string description,
			decimal min,
			decimal max,
			decimal increment = 1M)
		{
			Name = name;
			Description = description;
			IsDecimal = true;
			DecimalMin = min;
			DecimalMax = max;
			DecimalIncrement = increment;
		}
	}
}