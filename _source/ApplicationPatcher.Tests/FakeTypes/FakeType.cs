using System;

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeType {
		public readonly Type Type;
		public readonly string Name;
		public readonly string FullName;

		public FakeType(Type type) {
			Type = type;
			Name = type.Name;
			FullName = type.FullName;
		}
		public FakeType(string name) {
			Name = name;
			FullName = name;
		}
	}
}