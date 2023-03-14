using System.Data;
using UnityEngine;

/// <summary>
///     Lays out every card in a database so that it can be inspected
/// </summary>
/// <author>Joshua Dahl</author>
public class DatabaseZoo : MonoBehaviour {
    /// <summary>
    ///     The database to visualize
    /// </summary>
    public CardDatabase database;

    /// <summary>
    ///     The number of columns to split cards into
    /// </summary>
    public int numberOfColumns = 10;

    /// <summary>
    ///     At the start of the game instantiate a copy of every card in the database
    /// </summary>
    public void Start() {
		// Initialize x and y to keep track of the grid layout
		int x = 0, y = 0;

		// Loop through each card in the database
		foreach (var (name, card) in database.cards) {
			// Calculate the bounds of the card object and its children
			var position = GetRenderBounds(card.gameObject).extents * 2;
			position.z = 0;

			// Set the position of the card object based on its index in the grid
			position.x *= x * 2;
			position.y *= y * 2;

			// Instantiate a copy of the card object from the database
			var instantiated = database.Instantiate(name, position, card.transform.rotation);

			// Throw an exception if the card object could not be loaded from the database
			if (instantiated is null) throw new DataException($"Card {name} failed to be loaded from the database!");

			// Update the grid layout counters
			x++;
			if (x <= numberOfColumns) continue;
			x = 0;
			y++;
		}
	}

    /// <summary>
    ///     Function which calculates the visible bounding box of an object (and its children)
    /// </summary>
    /// <remarks>From: https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children</remarks>
    /// <param name="g">The game object to calculate the bounds for</param>
    /// <returns>The bounds of <paramref name="g" /> and its children</returns>
    private Bounds GetRenderBounds(GameObject g) {
		// Initialize a new bounding box centered at the game object's position
		var b = new Bounds(g.transform.position, Vector3.zero);

		// Loop through each renderer in the game object and its children
		foreach (var r in g.GetComponentsInChildren<Renderer>())
			// Encapsulate the renderer's bounding box in the overall bounding box
			b.Encapsulate(r.bounds);

		// Return the final bounding box
		return b;
	}
}