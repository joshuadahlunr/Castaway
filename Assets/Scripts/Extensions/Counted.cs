namespace Extensions {
	/// <summary>
	///     Represents a value and an associated count.
	/// </summary>
	/// <author>Joshua Dahl</author>
	/// <typeparam name="T">The type of the value.</typeparam>
	public class Counted<T> {
		/// <summary>
		///     Holds the value.
		/// </summary>
		public T value;

		/// <summary>
		///     Holds the count of how many times the value occurs.
		/// </summary>
		public int count;
	}
}