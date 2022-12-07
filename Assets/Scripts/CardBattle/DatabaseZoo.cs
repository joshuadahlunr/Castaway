using System.Data;
using UnityEngine;

// Lays out every card in the database so that it can be inspected
public class DatabaseZoo : MonoBehaviour {
    // The database to visualize
    public CardDatabase database;
    // The number of columns to split cards into
    public int numberOfColumns = 10;
    
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
    
    // From: https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children
    Bounds GetRenderBounds(GameObject g) {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>()) {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
}
