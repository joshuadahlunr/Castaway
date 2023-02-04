using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace CardBattle {
	[CreateAssetMenu(fileName = "PeopleJuice", menuName = "ScriptableObjects/PeopleJuice Mapping")]
	public class PeopleJuice : ScriptableObject {
		/// <summary>
		/// List enumerating the different types of people juice
		/// </summary>
		[Serializable]
		public enum Types {
			Generic,
			Wizard,
			Navigator,
			Entertainer,
			Engineer,
			Cook,
			Occultist,
			Mercenary,
			Deckhand,
			Max
		}

		/// <summary>
		/// Type representing a people juice cost (just a list of people juice)
		/// </summary>
		[Serializable]
		public class Cost : IEnumerable<Types> {
			[SerializeField] private List<Types> list;

			public int Count => list.Count;
			public int Value => Count;

			public Cost() => list = new List<Types>();
			public Cost(IEnumerable<Types> iter) => list = new List<Types>(iter);

			public Types this[int index] {
				get => list[index];
				set => list[index] = value;
			}
			
			public IEnumerator<Types> GetEnumerator() => list.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerable<Counted<Types>> CountedForm() {
				return list.GroupBy(t => t)
					.Select(g => new Counted<Types> { value = g.Key, count = g.Count() });
			}
			
			public static Cost ExpandedForm(IEnumerable<Counted<Types>> counted) {
				var @out = new Cost();
				foreach(var count in counted)
					for(var i = 0; i < count.count; i++)
						@out.list.Add(count.value);
				return @out;
			}

			public override string ToString() {
				return list.Aggregate("", (current, type) => current + $"{{{type}}} ");
			}

			public Cost Sort() {
				list.Sort();
				return this;
			}

			public Cost Add(Types symbol) {
				list.Add(symbol);
				return this;
			}

			public Cost AddRange(IEnumerable<Types> symbols) {
				list.AddRange(symbols);
				return this;
			}
		}
		
		/// <summary>
		/// Type which matches a people juice type to an associated icon
		/// </summary>
		[Serializable]
		public class PeopleJuiceIconsDictionary : SerializableDictionaryBase<Types, Sprite> { }

		/// <summary>
		/// Dictionary mapping the types of people juice to an associated sprite
		/// </summary>
		public PeopleJuiceIconsDictionary sprites;
		
		
		
		// ----- Static Helpers ------


		/// <summary>
		///     Deducts <see cref="cost" /> from the provided <see cref="pool" />
		/// </summary>
		/// <param name="pool">
		///     The pool to deduct a <see cref="cost" /> from. Passed as reference the variable in the caller will
		///     be holding the new pool once finished
		/// </param>
		/// <param name="cost">The cost to deduct from the <see cref="pool" /></param>
		/// <returns>
		///     True if the pool has enough value (of the correct types) to deduct the cost, false if there isn't enough value in
		///     the pool.
		/// </returns>
		public static bool DeductCost(ref Cost pool_, Cost cost_) {
			// Optimization, if there are more symbols in the cost than the pool... of course the pool can't afford the cost!
			if (pool_.Value < cost_.Value) return false;

			Counted<Types> needed;
			var pool = pool_.CountedForm().ToArray();
			var cost = cost_.CountedForm().ToArray();

			// Check if all the typed symbols are accounted for
			// For each type of symbol...
			for (var i = (int)Types.Generic + 1; i < (int)Types.Max; i++) {
				// Check if any of this type is needed
				needed = cost.FirstOrDefault(x => (int)x.value == i);
				if (needed == null) continue;

				// If it is needed check if any is available
				var available = pool.WithIndex().FirstOrDefault(x => (int)x.item.value == i);
				// available = [(x[1], j) for j, x in enumerate(pool) if x[0].value == i]
				if (available.item is null) return false; // If not available, cost isn't available in pool
				
				// Cost is available in pool if we need more than is available
				if (available.item.count < needed.count) return false;

				// Remove the typed cost from pool
				var tup = pool[available.index];
				tup.count -= needed.count;
				pool[available.index] = tup;
			}

			// The typed cost has now been deducted from the pool
			var ePool = Cost.ExpandedForm(pool); // Convert the pool to an expanded form
			ePool.Sort();

			// Determine if any generic cost is needed
			needed = cost.FirstOrDefault(x => x.value == Types.Generic);
			// needed = [x[1] for x in cost if x[0].value == Types.GENERIC.value]
			if (needed == null) {
				pool_ = ePool; // If no generic cost is needed, whats currently in the pool is finalized 
				return true;
			}

			// If some generic cost is needed, make sure the needed cost is less than whatever is remaining in the pool
			if (needed.count > ePool.Value) return false;

			// Just drop symbols from the front of the pool to pay the generic cost
			pool_ = new Cost(
				ePool.Skip(needed.count)); // TODO: Would draining the fullest pool first be better? round robin?
			return true;
		}

		/// <summary>
		///     Checks if there is enough value in the <see cref="pool" /> to deduct the provided <see cref="cost" />
		/// </summary>
		/// <param name="pool">The pool to check if the <see cref="cost" /> could be deducted from.</param>
		/// <param name="cost">The cost to hypothetically deduct from the <see cref="pool" /></param>
		/// <returns>
		///     True if the pool has enough value (of the correct types) to deduct the cost, false if there isn't enough value in
		///     the pool.
		/// </returns>
		public static bool CostAvailable(Cost pool, Cost cost) {
			var clone = new Cost(pool);
			return DeductCost(ref clone, cost);
		}
	}
}
