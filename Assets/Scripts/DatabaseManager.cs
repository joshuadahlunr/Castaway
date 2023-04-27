using System.Linq;
using CardBattle.Containers;
using Crew;
using SQLite;
using UnityEngine;

/// <summary>
/// Singleton manager which provides access to the SQL database
/// </summary>
/// <author>Joshua Dahl</author>
public static class DatabaseManager {
	/// <summary>
	/// Path to where the database can be located
	/// </summary>
	public static string DatabasePath => Application.persistentDataPath + "/castaway.db";

	/// <summary>
	/// Backing memory for the database connection
	/// </summary>
	private static SQLiteConnection _database;

	/// <summary>
	/// Read only property that ensures a connection to the database is created the first time it is requested
	/// </summary>
	public static SQLiteConnection database {
		get {
			SQLiteConnection CreateConnection() {
				Debug.Log($"Established a connection with the database located at {DatabasePath}");
				var db = new SQLiteConnection(DatabasePath);
				db.EnableWriteAheadLogging();
				return db;
			}

			_database ??= CreateConnection();
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

	/// <summary>
	/// Resets the database for a new "Save"
	/// </summary>
	public static void ResetToNewSave() {
		// Marks all the crewmates as no longer being in the deck
		CrewManager.SetAllCrewToFormer();

		// Removes the decklist table from the database
		database.DropTable<Deck.DeckListCard>();
	}

	/// <summary>
	/// Bool indicating if a save currently exists
	/// </summary>
	public static bool SaveExists => GetOrCreateTable<Deck.DeckListCard>().Any();
}