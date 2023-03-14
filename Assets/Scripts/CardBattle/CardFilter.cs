using System;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;

namespace CardBattle {
	/// <summary>
	///     A static class for filtering card objects based on various criteria.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public static class CardFilterer {
		/// <summary>
		///     An enum representing various card filters that can be applied.
		/// </summary>
		[Flags]
		public enum CardFilters {
			None = 0, // Default filter (no filters applied)
			Enemy = 1 << 1, // Filter for enemy-owned cards
			Player = 1 << 2, // Filter for player-owned cards
			Hand = 1 << 3, // Filter for cards in the player's hand
			InPlay = 1 << 4, // Filter for cards currently in play
			MonsterImpl = 1 << 5, // Filter for monster cards (internal use only)
			Monster = Enemy | MonsterImpl, // Filter for enemy-owned monster cards
			EquipmentImpl = 1 << 6, // Filter for equipment cards (internal use only)
			Equipment = EquipmentImpl | Player, // Filter for player-owned equipment cards
			Action = 1 << 7, // Filter for action cards
			Status = 1 << 8, // Filter for status cards
			Affordable = 1 << 9, // Filter for cards that can be afforded
			Unaffordable = 1 << 10, // Filter for cards that cannot be afforded

			/// <summary>
			///     A filter that matches all cards.
			/// </summary>
			All = ~0
		}

		/// <summary>
		///     A shortcut for accessing the CardGameManager instance.
		/// </summary>
		private static CardGameManager cgm => CardGameManager.instance;

		/// <summary>
		///     Enumerates all cards currently in play.
		/// </summary>
		/// <returns>An <see cref="IEnumerable" /> of all cards currently in play.</returns>
		public static IEnumerable<CardBase> EnumerateAllCards() {
			foreach (var card in cgm.playerHand) // Add all cards in the player's hand
				yield return card;
			foreach (var container in cgm.inPlayContainers) // Add all cards in play on both sides
				foreach (var card in container)
					yield return card;
			foreach (var card in cgm.monsters) // Add all monster cards
				yield return card;
		}

		/// <summary>
		///     Enumerates all disabled cards.
		/// </summary>
		/// <returns>An <see cref="IEnumerable" /> of all disabled cards.</returns>
		public static IEnumerable<CardBase> EnumerateDisabledCards() => EnumerateAllCards().Where(c => c.Disabled);

		/// <summary>
		///     Enumerates all enabled cards.
		/// </summary>
		/// <returns>An <see cref="IEnumerable" /> of all enabled cards.</returns>
		public static IEnumerable<CardBase> EnumerateEnabledCards() => EnumerateAllCards().Where(c => !c.Disabled);

		/// <summary>
		///     Filters a collection of cards based on the specified filters.
		/// </summary>
		/// <param name="filters">The filters to apply to the cards.</param>
		/// <param name="cards">The collection of cards to filter.</param>
		/// <returns>
		///     A tuple containing two arrays of cards: the first contains the cards that match the filters,
		///     and the second contains the cards that were filtered out.
		/// </returns>
		private static (CardBase[], CardBase[]) FilterCardsImpl(CardFilters filters, IEnumerable<CardBase> cards) {
			var matchingCards = new List<CardBase>();
			var filteredCards = new List<CardBase>();

			// Iterate through each card in the collection.
			foreach (var card in cards) {
				// If the Enemy filter is set and the card is owned by an enemy, add it to the filtered list.
				if ((filters & CardFilters.Enemy) != 0)
					if (!card.OwnedByPlayer) {
						filteredCards.Add(card);
						continue;
					}

				// If the Player filter is set and the card is owned by the player, add it to the filtered list.
				if ((filters & CardFilters.Player) != 0)
					if (card.OwnedByPlayer) {
						filteredCards.Add(card);
						continue;
					}

				// If the Hand filter is set and the card is in the player's hand, add it to the filtered list.
				if ((filters & CardFilters.Hand) != 0)
					if ((card.state & CardBase.State.InHand) != 0) {
						filteredCards.Add(card);
						continue;
					}

				// If the InPlay filter is set and the card is in play, add it to the filtered list.
				if ((filters & CardFilters.InPlay) != 0)
					if ((card.state & CardBase.State.InPlay) != 0) {
						filteredCards.Add(card);
						continue;
					}

				// If the MonsterImpl filter is set and the card is a monster card, add it to the filtered list.
				if ((filters & CardFilters.MonsterImpl) != 0)
					if (card is MonsterCardBase) {
						filteredCards.Add(card);
						continue;
					}

				// If the EquipmentImpl filter is set and the card is an equipment card, add it to the filtered list.
				if ((filters & CardFilters.EquipmentImpl) != 0)
					if (card is EquipmentCardBase) {
						filteredCards.Add(card);
						continue;
					}

				// If the Action filter is set and the card is an action card, add it to the filtered list.
				if ((filters & CardFilters.Action) != 0)
					if (card is ActionCardBase) {
						filteredCards.Add(card);
						continue;
					}

				// If the Status filter is set and the card is a status card, add it to the filtered list.
				if ((filters & CardFilters.Status) != 0)
					if (card is StatusCardBase) {
						filteredCards.Add(card);
						continue;
					}

				// If the card is an action card, check if it can be afforded.
				if (card is ActionCardBase aCard) {
					// If the affordable filter is set and the card is affordable, add it to the filtered list.
					if ((filters & CardFilters.Affordable) != 0)
						if (PeopleJuice.CostAvailable(cgm.currentPeopleJuice, aCard.cost)) {
							filteredCards.Add(card);
							continue;
						}

					// If the unaffordable filter is set and the card is unaffordable, add it to the filtered list.
					if ((filters & CardFilters.Unaffordable) != 0) // If filtering unaffordable cards
						if (!PeopleJuice.CostAvailable(cgm.currentPeopleJuice, aCard.cost)) {
							// Disable all cards that can't be afforded
							filteredCards.Add(card);
							continue;
						}
				}

				// If the card didn't get hit by any of the filters...
				// Add it to the list of matching cards
				matchingCards.Add(card);
			}

			// Return the complete list of matching cards
			return (matchingCards.ToArray(), filteredCards.ToArray());
		}

		/// <summary>
		///     Filters and disables the cards in the provided <paramref name="cards" /> collection according to the given <paramref name="filters" />.
		/// </summary>
		/// <param name="filters">The filters to use for the card filtering.</param>
		/// <param name="cards">The collection of cards to filter and disable.</param>
		/// <returns>An array of the filtered cards that were not disabled.</returns>
		public static CardBase[] FilterAndDisableCards(CardFilters filters, IEnumerable<CardBase> cards) {
			var lists = FilterCardsImpl(filters, cards);
			CardGameManager.DisableCards(lists.Item2);
			return lists.Item1;
		}

		/// <summary>
		///     Filters and disables all available cards according to the given <paramref name="filters" />.
		/// </summary>
		/// <param name="filters">The filters to use for the card filtering.</param>
		/// <returns>An array of the filtered cards that were not disabled.</returns>
		public static CardBase[] FilterAndDisableCards(CardFilters filters)
			=> FilterAndDisableCards(filters, EnumerateAllCards());

		/// <summary>
		///     Filters the cards in the provided <paramref name="cards" /> collection according to the given <paramref name="filters" />.
		/// </summary>
		/// <param name="filters">The filters to use for the card filtering.</param>
		/// <param name="cards">The collection of cards to filter.</param>
		/// <returns>An array of the filtered cards.</returns>
		public static CardBase[] FilterCards(CardFilters filters, IEnumerable<CardBase> cards)
			=> FilterCardsImpl(filters, cards).Item1;

		/// <summary>
		///     Filters all available cards according to the given <paramref name="filters" />.
		/// </summary>
		/// <param name="filters">The filters to use for the card filtering.</param>
		/// <returns>An array of the filtered cards.</returns>
		public static CardBase[] FilterCards(CardFilters filters) => FilterCards(filters, EnumerateAllCards());
	}
}