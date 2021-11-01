using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphDB_Testing.Gremlin
{
	/// <summary>
	/// Defines an edge to use in a graph database with Gremlin.
	/// </summary>
	public class GremlinEdge
	{
		public string Id { get; internal set; }
		public string Label { get; internal set; }
		public object InVertexId { get; internal set; }
		public object OutVertexId { get; internal set; }
		public string InVertexLabel { get; internal set; }
		public string OutVertexLabel { get; internal set; }
		public object InVertexPartitionKey { get; internal set; }
		public object OutVertexPartitionKey { get; internal set; }
		internal GremlinPropertyCollection Properties { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GremlinEdge"/> class.
		/// </summary>
		/// <param name="edgeId">The ID of the edge.</param>
		/// <param name="edgeLabel">The label for the edge.</param>
		/// <param name="outVertexId">The ID of the 'out' vertex.</param>
		/// <param name="inVertexId">The ID of the 'in' vertex.</param>
		/// <param name="outVertexLabel">The label of the 'out' vertex.</param>
		/// <param name="inVertexLabel">The label of the 'in' vertex.</param>
		/// <param name="outVertexPartitionKey">The 'out' partition key.</param>
		/// <param name="inVertexPartitionKey">The 'in' partition key.</param>
		public GremlinEdge(
			string edgeId, 
			string edgeLabel, 
			string outVertexId, 
			string inVertexId, 
			string outVertexLabel, 
			string inVertexLabel, 
			object outVertexPartitionKey = null, 
			object inVertexPartitionKey = null
		)
		{
			if (string.IsNullOrEmpty(edgeId))
			{
				throw new ArgumentException("Edge must have a valid Id.", nameof(edgeId));
			}

			if (string.IsNullOrEmpty(edgeLabel))
			{
				throw new ArgumentException("Edge must have a valid Label.", nameof(edgeLabel));
			}

			if (string.IsNullOrEmpty(inVertexId))
			{
				throw new ArgumentException("Edge must have a valid InVertexId.", nameof(inVertexId));
			}

			if (string.IsNullOrEmpty(outVertexId))
			{
				throw new ArgumentException("Edge must have a valid OutVertexId.", nameof(outVertexId));
			}

			if (string.IsNullOrEmpty(inVertexLabel))
			{
				throw new ArgumentException("Edge must have a valid InVertexLabel.", nameof(inVertexLabel));
			}

			if (string.IsNullOrEmpty(outVertexLabel))
			{
				throw new ArgumentException("Edge must specify OutVertexLabel.", nameof(outVertexLabel));
			}

			this.Id = edgeId;
			this.Label = edgeLabel;
			this.InVertexId = inVertexId;
			this.InVertexLabel = inVertexLabel;
			this.InVertexPartitionKey = inVertexPartitionKey;
			this.OutVertexId = outVertexId;
			this.OutVertexLabel = outVertexLabel;
			this.OutVertexPartitionKey = outVertexPartitionKey;
		}

		/// <summary>
		/// Gets the property for the given key.
		/// </summary>
		/// <param name="key">The key to get the property for.</param>
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
		/// Gets the properties for this Edge.
		/// </summary>
		/// <returns>The properties for this Edge.</returns>
		public IEnumerable<GremlinProperty> GetProperties()
		{
			if (Properties == null)
			{
				return Enumerable.Empty<GremlinProperty>();
			}
			return Properties;
		}

		/// <summary>
		/// Adds a property to this Edge.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		/// <returns>The modified Edge.</returns>
		public GremlinEdge AddProperty(string key, object value)
		{
			if (Properties == null)
			{
				Properties = new GremlinPropertyCollection();
			}

			Properties.Add(new GremlinProperty(key, value));
			return this;
		}
	}
}
