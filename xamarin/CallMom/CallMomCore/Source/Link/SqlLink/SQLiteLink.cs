using System;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;

namespace CallMomCore
{
	public class SQLiteLink : ISQLiteLink
	{

		private readonly SQLiteConnection _dbConnection;
		private static readonly object SqlLock = new object ();

		public SQLiteLink (ISQLiteFactory factory)
		{ 
			_dbConnection = new SQLiteConnection (factory.Platform (), factory.DatabasePath (), true);
		}

		#region ISQLiteLink implementation

		public void InsertOrUpdateItem<T> (T item) where T : class, new()
		{
			lock (SqlLock) {
				try {
					int ret = _dbConnection.InsertOrReplace (item);
					if (ret < 1) {
						throw new MomSqlException ("Could not insert item");
					}
				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "InsertOrUpdateItem");
				}
			}
		}

		public void RemoveItem<T> (int id) where T : class, new()
		{
			lock (SqlLock) {
				try {
					int ret = _dbConnection.Delete<T> (id);
					if (ret < 1) {
						throw new MomSqlException ("Could not insert item");
					}
				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "RemoveItem");
				}
			}
		}

		public T GetItemById<T> (int id) where T : class, new()
		{
			lock (SqlLock) {
				try {
					return _dbConnection.Get<T> (id);
				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "GetItemById");
				}
			}
		}

		public List<T> GetAllItems<T> () where T : class, new()
		{
			lock (SqlLock) {
				try {
					return _dbConnection.Table<T> ().ToList ();
				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "GetAllItems");
				}
			}
		}

		public int ResetTable<T> () where T : class, new()
		{
			lock (SqlLock) {
				try {
					int ret = _dbConnection.DropTable<T> ();
					ret += _dbConnection.CreateTable<T> ();
					return ret;
				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "ResetTable");
				}
			}
		}

		public void CleanupDB ()
		{
			lock (SqlLock) {
				try {

				} catch (Exception ex) {
					throw MomSqlException.ToMomException (ex, "CleanupDB");
				}
			}
		}


		public SQLiteConnection DB ()
		{
			return _dbConnection;
		}

		#endregion
	}
}

