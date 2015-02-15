using System;
using SQLite.Net.Interop;

namespace CallMomCore
{
	public interface ISQLiteFactory
	{
		/**
		 * Returns a string containing the path for the database
		 * @return - string
		 */
		string DatabasePath ();

		/**
		 * Returns the platform for the current used OS
		 * @return - ISQLitePlatform object
		 */
		ISQLitePlatform Platform ();
	}
}

