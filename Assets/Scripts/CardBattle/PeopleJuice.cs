using System;
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
			Generic	
		}

		/// <summary>
		/// Type representing a people juice cost (just a list of people juice)
		/// </summary>
		[Serializable]
		public class Cost : List<Types> {
			public IEnumerable<Counted<Types>> CountedForm() {
				return this.GroupBy(t => t)
					.Select(g => new Counted<Types> { value = g.Key, count = g.Count() });
			}
			
			public override string ToString() {
				string @out = "";
				foreach(var counted in CountedForm())
					@out += $"{{{counted.value}}} = {counted.count} ";
				return @out;
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


		// public static var 
		//
		//
		// bool CostAvailable(Cost pool, Cost cost) {
		// 	pool.Sort();
		// 	
		// }

	}
}