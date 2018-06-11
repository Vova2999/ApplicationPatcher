using System;

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeProperty {
		public readonly string Name;
		public readonly FakeMethod GetMethod;
		public readonly FakeMethod SetMethod;
		public readonly FakeType PropertyType;
		public readonly FakeAttribute[] Attributes;

		public FakeProperty(string name, Type propertyType, PropertyMethods propertyMethods, FakeAttribute[] attributes = null) : this(name, new FakeType(propertyType), propertyMethods, attributes) {
		}
		public FakeProperty(string name, FakeType propertyType, PropertyMethods propertyMethods, FakeAttribute[] attributes = null) {
			Name = name;
			PropertyType = propertyType;
			Attributes = attributes;

			switch (propertyMethods) {
				case PropertyMethods.HasGetOnly:
					GetMethod = new FakeMethod($"get_{name}", null);
					break;
				case PropertyMethods.HasSetOnly:
					SetMethod = new FakeMethod($"set_{name}", new[] { new FakeParameter("value", propertyType) });
					break;
				case PropertyMethods.HasGetAndSet:
					GetMethod = new FakeMethod($"get_{name}", null);
					SetMethod = new FakeMethod($"set_{name}", new[] { new FakeParameter("value", propertyType) });
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(propertyMethods), propertyMethods, null);
			}
		}
	}

	public enum PropertyMethods {
		HasGetOnly,
		HasSetOnly,
		HasGetAndSet
	}
}