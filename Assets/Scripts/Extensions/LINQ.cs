using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Extensions {
	
	// Additions to Linq
	public static class Linq {
		// Function that returns an iterator with the array index attached
		public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> data) {
			return data.Select((item, index) => (item, index));
		}

		// Function that flattens a container of containers into an array, returning the flattened array and another array indicating where each of the original arrays starts.
		// This allows very simple preparation of arrays of arrays of data for handoff to Unity Jobs
		public static void ConcatenateWithStartIndices<T>([NotNull] this IEnumerable<IEnumerable<T>> data, out T[] concatenatedData, out long[] starts) {
			starts = new long[data.LongCount()];
			starts[0] = 0;

			var concat = data.First();
			foreach (var (enumerable, i) in data.WithIndex().Skip(1)) {
				starts[i] = concat.LongCount();
				concat = concat.Concat(enumerable);
			}

			concatenatedData = concat.ToArray();
		}

		// Create an infinitely repeating view of the data stored within the range 
		public static IEnumerable<T> Replicate<T>([NotNull] this IEnumerable<T> data, bool indicator = true) {
			using var enumerator = data.GetEnumerator();
			while (true) {
				var shouldReset = enumerator.MoveNext();
				yield return enumerator.Current;
				if(shouldReset) enumerator.Reset();
			}
		}

		// Create a view of the data stored within the range that repeats <times> times 
		public static IEnumerable<T> Replicate<T>([NotNull] this IEnumerable<T> data, int times) {
			return data.Replicate().Take(data.Count() * times);
		}
	}
}