using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Extensions {
	/// <summary>
	///     Provides extension methods for working with IEnumerable collections.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public static class Linq {
		/// <summary>
		///     Returns an iterator with the array index attached for each item in the input sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
		/// <param name="data">The input sequence.</param>
		/// <returns>An IEnumerable of tuples where each tuple contains an item from the input sequence and its index.</returns>
		public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> data) {
			return data.Select((item, index) => (item, index));
		}

		/// <summary>
		///     Flattens a container of containers into an array, and returns the flattened array and an array indicating where each of the original arrays starts.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the input containers.</typeparam>
		/// <param name="data">The input containers to concatenate.</param>
		/// <param name="concatenatedData">The flattened array of elements from the input containers.</param>
		/// <param name="starts">An array of long values indicating the starting index of each original container within the flattened array.</param>
		public static void ConcatenateWithStartIndices<T>([NotNull] this IEnumerable<IEnumerable<T>> data,
			out T[] concatenatedData, out long[] starts) {
			starts = new long[data.LongCount()];
			starts[0] = 0;

			var concat = data.First();
			foreach (var (enumerable, i) in data.WithIndex().Skip(1)) {
				starts[i] = concat.LongCount();
				concat = concat.Concat(enumerable);
			}

			concatenatedData = concat.ToArray();
		}

		/// <summary>
		///     Returns an infinitely repeating view of the data stored within the input sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
		/// <param name="data">The input sequence.</param>
		/// <param name="indicator">If true, the enumerator will reset after reaching the end of the sequence.</param>
		/// <returns>An IEnumerable that returns the elements of the input sequence repeatedly.</returns>
		public static IEnumerable<T> Replicate<T>([NotNull] this IEnumerable<T> data, bool indicator = true) {
			using var enumerator = data.GetEnumerator();
			while (true) {
				var shouldReset = enumerator.MoveNext();
				yield return enumerator.Current;
				if (shouldReset && indicator) enumerator.Reset();
			}
		}

		/// <summary>
		///     Returns a view of the data stored within the input sequence that repeats a specified number of times.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
		/// <param name="data">The input sequence.</param>
		/// <param name="times">The number of times to repeat the sequence.</param>
		/// <returns>An IEnumerable that returns the elements of the input sequence repeated <paramref name="times" /> times.</returns>
		public static IEnumerable<T> Replicate<T>([NotNull] this IEnumerable<T> data, int times)
			=> data.Replicate().Take(data.Count() * times);

		/// <summary>
		///     Shuffles the elements of an <see cref="IEnumerable{T}" /> using the Fisher-Yates shuffle algorithm
		///     and returns the results as a new <see cref="List{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the input sequence.</typeparam>
		/// <param name="list">The input sequence to shuffle.</param>
		/// <returns>A new <see cref="List{T}" /> with the elements of the input sequence shuffled randomly.</returns>
		/// <remarks>
		///     From: https://www.programmingnotes.org/7398/cs-how-to-shuffle-randomize-an-array-list-ienumerable-using-cs/
		/// </remarks>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list) {
			var shuffled = list.ToList();
			for (var index = 0; index < shuffled.Count; ++index) {
				var randomIndex = Random.Range(index, shuffled.Count);
				if (index == randomIndex) continue;
				(shuffled[index], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[index]);
			}

			return shuffled;
		}
	}
}