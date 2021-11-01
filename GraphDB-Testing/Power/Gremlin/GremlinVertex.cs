using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphDB_Testing.Gremlin
{
	/// <summary>
	/// Defines a vertex to use in a graph database with Gremlin.
	/// </summary>
	public sealed class GremlinVertex
	{
		public string Id { get; internal set; }
		public string Label { get; internal set; }

		internal Dictionary<string, List<GremlinVertexProperty>> Properties { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GremlinVertex"/> class.
		/// </summary>
		/// <param name="id">The ID of the vertex.</param>
		/// <param name="label">The label of the vertex.</param>
		public GremlinVertex(string id, string label)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Vertex must have a valid Id.", nameof(id));
			}

			if (string.IsNullOrEmpty(label))
			{
				throw new ArgumentException("Vertex must have a valid Label.", nameof(label));
			}

			this.Id = id;
			this.Label = label;
		}

		/// <summary>
		/// Adds the given property to the vertex.
		/// </summary>
		/// <param name="key">The key of the property.</param>
		/// <param name="value">The value for the property.</param>
		public void AddProperty(string key, object value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("VertexProperty must have a valid Key.", nameof(key));
			}

			if (value == null)
			{
				throw new ArgumentException("VertexProperty must not have a null Value.", nameof(value));
			}

			if (Properties == null)
			{
				Properties = new Dictionary<string, List<GremlinVertexProperty>>();
			}

            if (!Properties.TryGetValue(key, out List<GremlinVertexProperty> propertiesForKey))
            {
                propertiesForKey = new List<GremlinVertexProperty>();
                Properties.Add(key, propertiesForKey);
            }

            propertiesForKey.Add(new GremlinVertexProperty(key, value));
		}

		/// <summary>
		/// Adds the given property to the vertex.
		/// </summary>
		/// <param name="vertexProperty">The property to add.</param>
		/// <returns>The added property.</returns>
		public GremlinVertexProperty AddProperty(GremlinVertexProperty vertexProperty)
		{
			if (Properties == null)
			{
				Properties = new Dictionary<string, List<GremlinVertexProperty>>();
			}

            if (!Properties.TryGetValue(vertexProperty.Key, out List<GremlinVertexProperty> propertiesForKey))
            {
                propertiesForKey = new List<GremlinVertexProperty>();
                Properties.Add(vertexProperty.Key, propertiesForKey);
            }

            propertiesForKey.Add(vertexProperty);
			return vertexProperty;
		}

		/// <summary>
		/// Gets the keys of the properties on this vertex.
		/// </summary>
		/// <returns>The keys of the properties on this vertex.</returns>
		public IEnumerable<string> GetPropertyKeys()
		{
			if (Properties == null)
			{
				return Enumerable.Empty<string>();
			}

			return Properties.Keys;
		}

		/// <summary>
		/// Gets the properties of this vertex for the given key.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <returns>The properties that were fetched.</returns>
		public IEnumerable<GremlinVertexProperty> GetVertexProperties(string key)
		{
			if (key == null)
			{
				throw new Exception(string.Format("Graph Element: Vertex: Get Property {0}", "key"));
			}

			if (Properties == null)
			{
				return Enumerable.Empty<GremlinVertexProperty>();
			}

			return Helpers.GetNestedEnumerable(Properties, key);
		}

		/// <summary>
		/// Gets the properties of this vertex.
		/// </summary>
		/// <returns>The properties that were fetched.</returns>
		public IEnumerable<GremlinVertexProperty> GetVertexProperties()
		{
			if (Properties == null)
			{
				return Enumerable.Empty<GremlinVertexProperty>();
			}

			return Helpers.GetNestedEnumerable(Properties);
		}
	}
}
