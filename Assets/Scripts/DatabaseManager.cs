using SQLite;
using UnityEngine;

/// <summary>
/// Singleton manager which provides access to the SQL database
/// </summary>
/// <author>Joshua Dahl</author>
public static class DatabaseManager {
	/// <summary>
	/// Backing memory for the database connection
	/// </summary>
	private static SQLiteConnection _database;
	/// <summary>
	/// Read only property that ensures a connection to the database is created the first time it is requested
	/// </summary>
	public static SQLiteConnection database {
		get {
			_database ??= new SQLiteConnection(Application.persistentDataPath + "/castaway.db");
			return _database;
		}
	}
	
	/// <summary>
	/// Function which ensures that the requested table exists and returns a reference to it
	/// </summary>
	/// <typeparam name="T">Schema of to get (represents the table to search in)</typeparam>
	/// <returns>Reference to the table in the database which holds data matching the schema defined by <see cref="T"/></returns>
	public static TableQuery<T> GetOrCreateTable<T>() where T : new() {
		database.CreateTable<T>();
		return database.Table<T>();
	}
}
