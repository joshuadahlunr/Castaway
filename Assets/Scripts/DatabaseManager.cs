using SQLite;
using UnityEngine;

public static class DatabaseManager {
	private static SQLiteConnection _database;
	public static SQLiteConnection database {
		get {
			_database ??= new SQLiteConnection(Application.persistentDataPath + "/castaway.db");
			return _database;
		}
	}
	
	// Function which ensures that the requested table exists and returns a reference to it
	public static TableQuery<T> GetOrCreateTable<T>() where T : new() {
		database.CreateTable<T>();
		return database.Table<T>();
	}
}
