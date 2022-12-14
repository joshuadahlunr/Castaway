using System.Data;
using UnityEngine;

/// <summary>
/// Lays out every card in a database so that it can be inspected
/// </summary>
/// <author>Joshua Dahl</author>
public class DatabaseZoo : MonoBehaviour {
    /// <summary>
    /// The database to visualize
    /// </summary>
    public CardDatabase database;
    /// <summary>
    /// The number of columns to split cards into
    /// </summary>
    public int numberOfColumns = 10;
    
    /// <summary>
    /// At the start of the game instantiate a copy of every card in the database
    /// </summary>
    public void Start() {
        int x = 0, y = 0;

        foreach (var (name, card) in database.cards) {
            var position = GetRenderBounds(card.gameObject).extents * 2;
            position.z = 0;
            position.x *= x * 2;
            position.y *= y * 2;
            
            var instantiated = database.Instantiate(name, position, card.transform.rotation);
            if (instantiated is null) throw new DataException($"Card {name} failed to be loaded from the database!");
            
            // Position variable book keeping
            x++;
            if (x <= numberOfColumns) continue;
            x = 0;
            y++;
        }
    }
    
    
    /// <summary>
    /// Function which calculates the visible bounding box of an object (and its children>
    /// </summary>
    /// <remarks>From: https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children</remarks>
    /// <param name="g">The game object to bound</param>
    /// <returns>The bounds of <see cref="g"/> and its children</returns>
    Bounds GetRenderBounds(GameObject g) {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>()) {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
}
