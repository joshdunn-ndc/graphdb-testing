using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphDB_Testing.Gremlin
{
	/// <summary>
	/// Defines a vertex property.
	/// </summary>
	public sealed class GremlinVertexProperty
	{
		public object Id { get; internal set; }
		public string Key { get; internal set; }
		public object Value { get; internal set; }
		internal GremlinPropertyCollection Properties { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GremlinVertexProperty"/> class.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		public GremlinVertexProperty(string key, object value)
		{
			if (string.IsNullOrEmpty(key))
			{
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
			}

			if (value == null)
			{
				throw new ArgumentException("Value cannot be null.", nameof(value));
			}

			this.Key = key;
			this.Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GremlinVertexProperty"/> class.
		/// </summary>
		/// <param name="id">The ID of the property.</param>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		internal GremlinVertexProperty(object id, string key, object value)
		{
			this.Id = id;
			this.Key = key;
			this.Value = value;
		}

		/// <summary>
		/// Gets the property for the given key.
		/// </summary>
		/// <param name="key">The key to fetch the property for.</param>
		/// <returns>The property for the given key.</returns>
		public GremlinProperty GetProperty(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (!Properties.TryGetProperty(key, out var property))
			{
				return null;
			}

			return property;
		}

		/// <summary>
		/// Gets the properties for the vertex.
		/// </summary>
		/// <returns>The properties.</returns>
		public IEnumerable<GremlinProperty> GetProperties()
		{
			if (Properties == null)
			{
				return Enumerable.Empty<GremlinProperty>();
			}

			return Properties;
		}

		/// <summary>
		/// Adds the given property data.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		/// <returns>This property.</returns>
		internal GremlinVertexProperty AddProperty(string key, object value)
		{
			if (Properties == null)
			{
				this.Properties = new GremlinPropertyCollection();
			}

			Properties.Add(new GremlinProperty(key, value));
			return this;
		}
	}

}
