using System.Collections.Generic;
using SQLite.Net;

namespace CallMomCore
{
	public interface ISQLiteLink
	{
		/// <summary>
		/// Inserts or updates an item.
		/// </summary>
		/// <param name="item">Item to insert.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="MomSqlException">If the insertion fails</exception>
		void InsertOrUpdateItem<T> (T item) where T : class, new();

		/// <summary>
		/// Removes an item by primary key.
		/// </summary>
		/// <param name="id">Primary key of the item to remove.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="MomSqlException">If the deletion fails</exception>
		void RemoveItem<T> (int id) where T : class, new();

		/// <summary>
		/// Gets an item by the primary key.
		/// </summary>
		/// <returns>The item by identifier.</returns>
		/// <param name="id">Primary key of the item to return.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="MomSqlException">If the returnation fails</exception>
		T GetItemById<T> (int id) where T : class, new();

		/// <summary>
		/// Gets all the items in a table.
		/// </summary>
		/// <returns>The all items as list.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="MomSqlException">If the returnation fails</exception>
		List<T> GetAllItems<T> () where T : class, new();

		/// <summary>
		/// Resets a table.
		/// </summary>
		/// <returns>2 if successfull</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="MomSqlException">If the reset fails</exception>
		int ResetTable<T> () where T : class, new();

		/// <summary>
		/// Cleans up the database.
		/// </summary>
		void CleanupDB ();

		/// <summary>
		/// Returns the database connection
		/// </summary>
		SQLiteConnection DB ();
	}
}

