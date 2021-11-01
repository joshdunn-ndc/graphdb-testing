using GraphDB_Testing.Gremlin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphDB_Testing
{
	/// <summary>
	/// Defines helper methods for the application execution.
	/// </summary>
    public class Helpers
    {

        public static IEnumerable<T> GetNestedEnumerable<T>(Dictionary<string, List<T>> dictionary)
        {
            return dictionary.Keys.SelectMany((string k) => GetNestedEnumerable(dictionary, k));
        }


		public static IEnumerable<T> GetNestedEnumerable<T>(Dictionary<string, List<T>> dictionary, string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			List<T> valueList = dictionary[key];

			foreach (T item in valueList)
			{
				yield return item;
			}
		}


		public static string GetVertexDocumentString(
			GremlinVertex vertex, 
			string partitionKey, 
			bool isFlatProperty, 
			bool isPartitionedCollection
		)
		{
			StringBuilder builder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(builder);
			bool isPartitionKeyProvided = false;
			JsonWriter writer = new JsonTextWriter(stringWriter);

			try
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();
				writer.WritePropertyName("id");
				writer.WriteValue(vertex.Id);
				writer.WritePropertyName("label");
				writer.WriteValue(vertex.Label);

				foreach (string key in vertex.GetPropertyKeys())
				{
					if (key.Equals(partitionKey))
					{
						GremlinVertexProperty vp = vertex.GetVertexProperties(key).FirstOrDefault();
						if (vp == null)
						{
							throw new ArgumentException("Partition key property can't be null");
						}

						writer.WritePropertyName(vp.Key);
						writer.WriteValue(vp.Value);
						isPartitionKeyProvided = true;
						continue;
					}

					if (isFlatProperty)
					{
						GremlinVertexProperty vp = vertex.GetVertexProperties(key).FirstOrDefault();
						if (vp == null)
						{
							throw new ArgumentException("Vertex property can't be null");
						}

						writer.WritePropertyName(vp.Key);
						writer.WriteValue(vp.Value);
						continue;
					}

					writer.WritePropertyName(key);
					writer.WriteStartArray();

					foreach (GremlinVertexProperty mvp in vertex.GetVertexProperties(key))
					{
						writer.WriteStartObject();
						writer.WritePropertyName("_value");
						writer.WriteValue(mvp.Value);
						writer.WritePropertyName("id");
						writer.WriteValue(Guid.NewGuid());
						writer.WriteEndObject();
					}

					writer.WriteEndArray();
				}

				if (isPartitionedCollection && !isPartitionKeyProvided)
				{
					throw new ArgumentException("PartitionKey property must be specified while adding a vertex to a partitioned graph.");
				}

				writer.WriteEndObject();
			}
			finally
			{
				((IDisposable)writer)?.Dispose();
			}
			return builder.ToString();
		}


		public static string GetEdgeDocumentString(
			GremlinEdge edge, 
			bool isPartitionedCollection, 
			string partitionKey, 
			HashSet<string> edgeSystemProperties
		)
		{
			StringBuilder builder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(builder);

			using (JsonWriter writer = new JsonTextWriter(stringWriter))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();
				writer.WritePropertyName("id");
				writer.WriteValue(edge.Id);
				writer.WritePropertyName("label");
				writer.WriteValue(edge.Label);
				writer.WritePropertyName("_sink");
				writer.WriteValue(edge.InVertexId);
				writer.WritePropertyName("_vertexId");
				writer.WriteValue(edge.OutVertexId);
				writer.WritePropertyName("_sinkLabel");
				writer.WriteValue(edge.InVertexLabel);
				writer.WritePropertyName("_vertexLabel");
				writer.WriteValue(edge.OutVertexLabel);
				writer.WritePropertyName("_isEdge");
				writer.WriteValue(value: true);

				if (isPartitionedCollection)
				{
					if (edge.InVertexPartitionKey == null)
					{
						throw new Exception("Edge must have a valid InVertexPartitionKey for a partitioned graph.");
					}

					if (edge.OutVertexPartitionKey == null)
					{
						throw new Exception("Edge must have a valid OutVertexPartitionKey for a partitioned graph.");
					}

					writer.WritePropertyName(partitionKey);
					writer.WriteValue(edge.OutVertexPartitionKey);
					writer.WritePropertyName("_sinkPartition");
					writer.WriteValue(edge.InVertexPartitionKey);
				}

				foreach (GremlinProperty ep in edge.GetProperties())
				{
					if (edgeSystemProperties.Contains(ep.Key))
					{
						throw new Exception($"Property: {ep.Key} is not allowed as an edge property.");
					}

					writer.WritePropertyName(ep.Key);
					writer.WriteValue(ep.Value);
				}

				writer.WriteEndObject();
			}
			return builder.ToString();
		}
	}
}
