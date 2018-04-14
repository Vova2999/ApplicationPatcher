using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ApplicationPatcher.Core.Helpers {
	internal static class XmlSerializerHelper {
		private static readonly XmlWriterSettings settings;
		private static readonly XmlSerializerNamespaces emptyNamespaces;
		private static readonly ConcurrentDictionary<Type, XmlSerializer> xmlSerializers;

		static XmlSerializerHelper() {
			settings = new XmlWriterSettings {
				Indent = true,
				IndentChars = "\t",
				OmitXmlDeclaration = false,
				Encoding = Encoding.UTF8
			};

			emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
			xmlSerializers = new ConcurrentDictionary<Type, XmlSerializer>();
		}

		public static byte[] Serializing(object obj) {
			var xmlSerializer = GetXmlSerializer(obj.GetType());

			using (var memoryStream = new MemoryStream())
			using (var xmlWriter = XmlWriter.Create(memoryStream, settings)) {
				xmlSerializer.Serialize(xmlWriter, obj, emptyNamespaces);
				return memoryStream.ToArray();
			}
		}

		public static TKey Deserializing<TKey>(byte[] bytes) {
			var xmlSerializer = GetXmlSerializer(typeof(TKey));

			using (var stream = new MemoryStream(bytes))
				return (TKey)xmlSerializer.Deserialize(stream);
		}

		private static XmlSerializer GetXmlSerializer(Type currentType) {
			return xmlSerializers.GetOrAdd(currentType, type => new XmlSerializer(type));
		}
	}
}