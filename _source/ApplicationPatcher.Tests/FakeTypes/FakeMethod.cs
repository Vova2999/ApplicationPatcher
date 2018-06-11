namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeMethod {
		public readonly string Name;
		public readonly FakeParameter[] Parameters;
		public readonly FakeAttribute[] Attributes;

		public FakeMethod(string name, FakeParameter[] parameters, FakeAttribute[] attributes = null) {
			Name = name;
			Parameters = parameters;
			Attributes = attributes;
		}
	}
}