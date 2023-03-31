using UnityEngine;

namespace CardBattle.Card {
	public class SelkieMonsterCardBase : MonsterCardBase {
		/// <summary>
		/// Modification that indicates that this card belongs to a selkie
		/// </summary>
		public class SelkieModification: ActionCardBase.Modification {
			public SelkieMonsterCardBase selkie;
			public override string GetName(string name) => selkie.name + "'s " + name;
		}

		protected Vector3 backupPosition, backupScale;
		protected Quaternion backupRotation;

		public void BecomeSelkie() {
			backupPosition = transform.position;
			backupRotation = transform.rotation;
			backupScale = transform.localScale;

			var sc = Instantiate(selkieCard);
			sc.AddModification(new SelkieModification {selkie = this});
			sc.associatedSelkie = this;
			transform.parent = sc.transform;
			CardGameManager.instance.playerDeck.AddCard(sc);
			CardGameManager.instance.playerDeck.Shuffle();
			gameObject.SetActive(false);
		}

		public void RestoreFromSelkie() {
			transform.parent = null;
			transform.position = backupPosition;
			transform.rotation = backupRotation;
			transform.localScale = backupScale;
			gameObject.SetActive(true);
			// Give a card other than become selkie a chance to be on the top of the deck
			deck.Shuffle();
		}
	}
}