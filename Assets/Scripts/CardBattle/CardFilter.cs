using System;
using System.Collections.Generic;
using System.Linq;

namespace CardBattle {
	public static class CardFilterer {

		[Flags]
		public enum CardFilters {
			None = 0,
			Enemy = 1 << 1,
			Player = 1 << 2,
			Hand = 1 << 3,
			InPlay = 1 << 4,
			MonsterImpl = 1 << 5,
			Monster = Enemy | MonsterImpl,
			EquipmentImpl = 1 << 6,
			Equipment = EquipmentImpl | Player,
			Action = 1 << 7,
			Status = 1 << 8,
			Affordable = 1 << 9,
			Unaffordable = 1 << 10,
			
			// Everything...
			All = ~0
		}

		private static CardGameManager cgm => CardGameManager.instance;
		
		public static IEnumerable<Card.CardBase> EnumerateAllCards() {
			foreach (var card in cgm.playerHand)
				yield return card;
			foreach(var container in cgm.inPlayContainers)
				foreach (var card in container)
					yield return card;
			foreach (var card in cgm.monsters)
				yield return card;
		}

		public static IEnumerable<Card.CardBase> EnumerateDisabledCards() =>
			EnumerateAllCards().Where(c => c.Disabled);
		
		public static IEnumerable<Card.CardBase> EnumerateEnabledCards() =>
			EnumerateAllCards().Where(c => !c.Disabled);

		private static (Card.CardBase[], Card.CardBase[]) FilterCardsImpl(CardFilters filters, IEnumerable<Card.CardBase> cards) {
			var matchingCards = new List<Card.CardBase>();
			var filteredCards = new List<Card.CardBase>();
			foreach (var card in cards) {
				if((filters & CardFilters.Enemy) != 0) { // If filtering enemies'
					if (!card.OwnedByPlayer) { // Disabled everything owned by enemies
						filteredCards.Add(card);
						continue;
					}
				}
				
				if ((filters & CardFilters.Player) != 0) { // If filtering players'
					if (card.OwnedByPlayer) { // Disable everything owned by the player
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.Hand) != 0) { // If filtering in Hand
					if ((card.state & Card.CardBase.State.InHand) != 0) { // Disable everything in hand!
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.InPlay) != 0) { // If filtering in play
					if ((card.state & Card.CardBase.State.InPlay) != 0) { // Disable everything in play!
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.MonsterImpl) != 0) { // If filtering monster cards
					if (card is Card.MonsterCardBase) { // Disable all monster cards
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.EquipmentImpl) != 0) { // If filtering equipment cards
					if (card is Card.EquipmentCardBase) { // Disable all equipment cards
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.Action) != 0) { // If filtering action cards
					if (card is Card.ActionCardBase) { // Disable all action cards
						filteredCards.Add(card);
						continue;
					}
				}
				
				if((filters & CardFilters.Status) != 0) { // If filtering status cards
					if (card is Card.StatusCardBase) { // Disable all status cards
						filteredCards.Add(card);
						continue;
					}
				}

				if (card is Card.ActionCardBase aCard) {
					if((filters & CardFilters.Affordable) != 0) { // If filtering affordable cards
						if (PeopleJuice.CostAvailable(cgm.currentPeopleJuice, aCard.cost)) { // Disable all cards that can be afforded
							filteredCards.Add(card);
							continue;
						}
					}
					
					if((filters & CardFilters.Unaffordable) != 0) { // If filtering unaffordable cards
						if (!PeopleJuice.CostAvailable(cgm.currentPeopleJuice, aCard.cost)) { // Disable all cards that can't be afforded
							filteredCards.Add(card);
							continue;
						}
					}
				}
				
				// If the card didn't get hit by any of the filters...
				// Add it to the list of matching cards
				matchingCards.Add(card);
			}

			// Return the complete list of matching cards
			return (matchingCards.ToArray(), filteredCards.ToArray());
		}

		public static Card.CardBase[] FilterAndDisableCards(CardFilters filters, IEnumerable<Card.CardBase> cards) {
			var lists = FilterCardsImpl(filters, cards);
			CardGameManager.DisableCards(lists.Item2);
			return lists.Item1;
		}

		public static Card.CardBase[] FilterAndDisableCards(CardFilters filters) => FilterAndDisableCards(filters, EnumerateAllCards());

		public static Card.CardBase[] FilterCards(CardFilters filters, IEnumerable<Card.CardBase> cards) =>
			FilterCardsImpl(filters, cards).Item1;

		public static Card.CardBase[] FilterCards(CardFilters filters) =>
			FilterCards(filters, EnumerateAllCards());
	}
}