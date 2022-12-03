using System;
using SQLite;
using UnityEngine;

public class SQLDatabaseTest : MonoBehaviour {
	public class Stock {
		[PrimaryKey, AutoIncrement] public int Id { get; set; }
		public string Symbol { get; set; }
	}

	public class Valuation {
		[PrimaryKey, AutoIncrement] public int Id { get; set; }
		[Indexed] public int StockId { get; set; }
		public DateTime Time { get; set; }
		public decimal Price { get; set; }
	}

	public void Start() {
		var db = DatabaseManager.database;
		db.CreateTable<Stock>();
		db.CreateTable<Valuation>();

		Debug.Log(Application.persistentDataPath + "/castaway.db");
		
		var stock = new Stock {
			Symbol = "ABC"
		};
		db.Insert(stock);
		Debug.Log($"{stock.Symbol} == {stock.Id}");

		var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));
		foreach (var s in query)
			Debug.Log("Stock: " + s.Symbol);
	}
}