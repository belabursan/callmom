using System;
using SQLite.Net.Interop;
using System.IO;
using SQLite.Net.Platform.XamarinIOS;
using UIKit;
using Foundation;
using CallMomCore;

namespace CallMomiOS
{
	public class SQLiteFactory : ISQLiteFactory
	{
		private string DbName { get; set; }

		public SQLiteFactory (string dbName)
		{
			DbName = dbName;
		}

		#region ISQLiteFactory implementation

		public string DatabasePath ()
		{
			return Path.Combine (DatabaseDirectory (), DbName);
		}

		public ISQLitePlatform Platform ()
		{
			return new SQLitePlatformIOS (); 
		}

		#endregion

		private string DatabaseDirectory ()
		{
			string dbDir = "";
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var docs = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
				dbDir = docs.Path;
			} else {
				dbDir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			}
			return dbDir;
		}
	}
}

