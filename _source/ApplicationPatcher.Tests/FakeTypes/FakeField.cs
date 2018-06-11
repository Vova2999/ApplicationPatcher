using System;

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeField {
		public readonly string Name;
		public readonly FakeType FieldType;
		public readonly FakeAttribute[] Attributes;

		public FakeField(string name, Type fieldType, FakeAttribute[] attributes = null) : this(name, new FakeType(fieldType), attributes) {
		}
		public FakeField(string name, FakeType fieldType, FakeAttribute[] attributes = null) {
			Name = name;
			FieldType = fieldType;
			Attributes = attributes;
		}
	}
}