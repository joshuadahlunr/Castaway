using Card;
using UnityEngine;

public class Hand : CardContainerBase {
	public class HandLayouter : MonoBehaviour {
		public Hand owner;
		public CardBase card;
		
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
	
	
	
	public override Facing facing => Facing.FaceUp;

	public void Awake() {
		foreach(var card in GetComponentsInChildren<CardBase>())
			AddCard(card);
	}

	public override void AddCard(Card.CardBase card, int index = -1) {
		base.AddCard(card, index);

		card.transform.localPosition = Vector3.zero;
		card.transform.localRotation = Quaternion.identity;

		// Mark the card as now being in hand
		card.state |= CardBase.State.InHand;
		// Add the hand layout component to the card
		var layout = card.gameObject.AddComponent<HandLayouter>();
		layout.owner = this;
		layout.card = card;
	}

	public override void RemoveCard(int index) {
		// Mark the card as no longer being in hand
		cards[index].state &= ~CardBase.State.InHand;
		// Remove the hand layout component to the card
		Destroy(cards[index].GetComponent<HandLayouter>());
		
		base.RemoveCard(index);
	}
}