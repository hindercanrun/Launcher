using System;

namespace Launcher
{
	public struct DVar
	{
		public string name;
		public string description;
		public bool isDecimal;
		public Decimal decimalMin;
		public Decimal decimalMax;
		public Decimal decimalIncrement;

		public DVar(string name, string description)
		{
			this.name = name;
			this.description = description;
			this.isDecimal = false;
			this.decimalMin = 0M;
			this.decimalMax = 0M;
			this.decimalIncrement = 0M;
		}

		public DVar(
			string name,
			string description,
			Decimal decimalMin,
			Decimal decimalMax,
			Decimal decimalIncrement)
		{
			this.name = name;
			this.description = description;
			this.isDecimal = true;
			this.decimalMin = decimalMin;
			this.decimalMax = decimalMax;
			this.decimalIncrement = decimalIncrement;
		}

		public DVar(string name, string description, Decimal decimalMin, Decimal decimalMax)
		{
			this.name = name;
			this.description = description;
			this.isDecimal = true;
			this.decimalMin = decimalMin;
			this.decimalMax = decimalMax;
			this.decimalIncrement = 1M;
		}
	}
}