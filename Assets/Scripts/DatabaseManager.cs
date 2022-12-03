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
}
