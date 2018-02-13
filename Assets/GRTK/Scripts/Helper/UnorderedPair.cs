using UnityEngine;
using System.Collections;
using System;

namespace GRTK
{
	/// <summary>
	/// An unordered pair that is equal regardless of the position of each object in the tuple
	/// </summary>
	/// <typeparam name="T">type of both objects (obviously not equal if different types!)</typeparam>
	public class UnorderedPair<T>
	{
		public T item1 { get; private set; }
		public T item2 { get; private set; }

		public UnorderedPair(T item1, T item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}

		// For hash sets
		public override int GetHashCode()
		{
			return item1.GetHashCode() * item2.GetHashCode();
		}

		// Follows MSDN spec
		public override bool Equals(object obj)
		{
			// If null or non castable, return false
			if (obj == null)
				return false;
			UnorderedPair<T> other = obj as UnorderedPair<T>;
			if (other == null)
			{
				return false;
			}

			return (item1.Equals(other.item1) && item2.Equals(other.item2)) || (item2.Equals(other.item1) && item1.Equals(other.item2));
		}

		// Follows MSDN spec
		public static bool operator ==(UnorderedPair<T> a, UnorderedPair<T> b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.Equals(b);
		}

		// Follows MSDN spec
		public static bool operator !=(UnorderedPair<T> a, UnorderedPair<T> b)
		{
			return !(a == b);
		}
	}

}