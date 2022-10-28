using System;
using UnityEngine;

namespace Card {
    // Base class all card types can inherit from!
    public class CardBase : MonoBehaviour {
        // List of valid states this card can be in
        [Flags]
        public enum State {
            None = 0,
            Inactive = 1 << 1,      // A card is in this state when it can't be seen/like in the deck
            InPlay = 1 << 2,        // When the card is on the field
            InHand = 1 << 3,        // When the card is in hand
        }
        // By default cards are inactive
        public State state = State.Inactive;
        
        public string name;
        public Texture2D art;
        public string rules;
        
        // TODO: Add renderer management stuff
        // TODO: Add some events which can be subscribed to in the editor (or broadcast system?)
    }
}

