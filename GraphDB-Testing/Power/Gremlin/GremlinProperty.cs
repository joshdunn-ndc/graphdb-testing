using System;

namespace GraphDB_Testing.Gremlin
{
	/// <summary>
	/// Defines a property to use against an Edge or Vertex in a graph database with Gremlin.
	/// </summary>
	public sealed class GremlinProperty : IEquatable<GremlinProperty>
	{
		public string Key { get; internal set; }
		public object Value { get; internal set; }

		internal GremlinProperty(string key, object value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("The property key cannot be null.", nameof(key));
			}

			if (value == null)
			{
				throw new ArgumentException("The property value cannot be null.", nameof(value));
			}

			this.Key = key;
			this.Value = value;
		}

		/// <summary>
		/// Determines if the given <see cref="GremlinProperty"/> is equal to this property.
		/// </summary>
		/// <param name="other">The other property to check.</param>
		/// <returns>True if they are equal, otherwise false.</returns>
		public bool Equals(GremlinProperty other)
		{
			if (other == null)
			{
				return false;
			}

			if (string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase) && Value.GetType() == other.Value.GetType())
			{
				return Value.Equals(other.Value);
			}

			return false;
		}
	}

}
