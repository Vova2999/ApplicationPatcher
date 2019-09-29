using System;
using JetBrains.Annotations;

namespace ApplicationPatcher.Tests.FakeTypes {
	[PublicAPI]
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