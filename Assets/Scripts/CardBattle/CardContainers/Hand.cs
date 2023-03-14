using UnityEngine;

namespace CardBattle.Containers {
	/// <summary>
	/// Container representing the player's hand
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Hand : CardContainerBase {

		/// <summary>
		/// Component added to cards in the hand which will automatically position them
		/// </summary>
		public class HandLayouter : MonoBehaviour {
			/// <summary>
			/// The hand which is currently managing the card
			/// </summary>
			public Hand owner;

			/// <summary>
			/// The card which is currently being managed
			/// </summary>
			public Card.CardBase card;

			/// <summary>
			/// Every frame position the card
			/// </summary>
			public void Update() {
				var camera = Camera.main;
				var depth = (owner.transform.position - camera?.transform.position ?? Vector3.zero).magnitude;
				var _min = camera?.ScreenToWorldPoint(new Vector3(0, 0, depth)).x ?? 0;
				var _max = camera?.ScreenToWorldPoint(new Vector3(Screen.width, 0, depth)).x ?? 0;

				var min = Mathf.Lerp(_min, _max, .1f);
				var max = Mathf.Lerp(_min, _max, .9f);

				// TODO improve placement algorithm to use an arc around the center of the screen!

				var pos = transform.position;
				pos.x = Mathf.Lerp(min, max, (float)owner.Index(card) / owner.Count);
				transform.position = pos;
			}
		}


		/// <summary>
		/// Hands are face up containers
		/// </summary>
		public override Facing facing => Facing.FaceUp;


		/// <summary>
		/// Override of AddCard that when cards are added to the hand, flags them as being in hand and adds a layout component to them!
		/// </summary>
		/// <param name="card">The card to be added</param>
		/// <param name="index">Optional index indicating where it should be inserted</param>
		public override void AddCard(Card.CardBase card, int index = -1) {
			base.AddCard(card, index);

			card.transform.localPosition = Vector3.zero;
			card.transform.localRotation = Quaternion.identity;

			// Mark the card as now being in hand
			card.state |= Card.CardBase.State.InHand;
			// Add the hand layout component to the card
			var layout = card.gameObject.AddComponent<HandLayouter>();
			layout.owner = this;
			layout.card = card;
		}

		/// <summary>
		/// Override of RemoveCard that removes the layout component and unflags the card as being in hand
		/// </summary>
		/// <param name="index">The index of the card to remove</param>
		public override void RemoveCard(int index) {
			// Mark the card as no longer being in hand
			cards[index].state &= ~Card.CardBase.State.InHand;
			// Remove the hand layout component to the card
			Destroy(cards[index].GetComponent<HandLayouter>());

			base.RemoveCard(index);
		}
	}
}