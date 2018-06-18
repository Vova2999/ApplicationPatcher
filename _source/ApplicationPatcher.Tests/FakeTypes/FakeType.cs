using System;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeType {
		public readonly Type Type;
		public readonly Type BaseType;
		public readonly string FullName;

		public FakeType(Type type) {
			Type = type;
			FullName = type.FullName;
			BaseType = type.BaseType;
		}
		public FakeType(string fullName, Type baseType = null) {
			FullName = fullName;
			BaseType = baseType ?? typeof(object);
		}
	}
}