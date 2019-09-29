using JetBrains.Annotations;

namespace ApplicationPatcher.Tests.FakeTypes {
	[PublicAPI]
	public class FakeMethod {
		public readonly string Name;
		public readonly FakeType ReturnType;
		public readonly FakeParameter[] Parameters;
		public readonly FakeAttribute[] Attributes;

		public FakeMethod(string name, FakeType returnType, FakeParameter[] parameters, FakeAttribute[] attributes = null) {
			Name = name;
			ReturnType = returnType;
			Parameters = parameters;
			Attributes = attributes;
		}
	}
}