using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ApplicationPatcher.Core.Helpers {
	internal static class XmlSerializerHelper {
		private static readonly XmlWriterSettings Settings;
		private static readonly XmlSerializerNamespaces EmptyNamespaces;
		private static readonly ConcurrentDictionary<Type, XmlSerializer> XmlSerializers;

		static XmlSerializerHelper() {
			Settings = new XmlWriterSettings {
				Indent = true,
				IndentChars = "\t",
				OmitXmlDeclaration = false,
				Encoding = Encoding.UTF8
			};

			EmptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
			XmlSerializers = new ConcurrentDictionary<Type, XmlSerializer>();
		}

		internal static byte[] Serializing(object obj) {
			var xmlSerializer = GetXmlSerializer(obj.GetType());

			using (var memoryStream = new MemoryStream())
			using (var xmlWriter = XmlWriter.Create(memoryStream, Settings)) {
				xmlSerializer.Serialize(xmlWriter, obj, EmptyNamespaces);
				return memoryStream.ToArray();
			}
		}

		internal static TKey Deserializing<TKey>(byte[] bytes) {
			var xmlSerializer = GetXmlSerializer(typeof(TKey));

			using (var stream = new MemoryStream(bytes))
				return (TKey)xmlSerializer.Deserialize(stream);
		}

		private static XmlSerializer GetXmlSerializer(Type currentType) {
			return XmlSerializers.GetOrAdd(currentType, type => new XmlSerializer(type));
		}
	}
}