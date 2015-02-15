using System;
using SQLite.Net.Attributes;

namespace CallMomCore
{
	[Table ("Settings")]
	public class Settings
	{
		[Column ("Key"), PrimaryKey, NotNull]
		public int Key { get; set; }

		[Column ("Value")]
		public string Value { get; set; }
	}
}

