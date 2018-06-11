namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeConstructor {
		public readonly FakeParameter[] Parameters;
		public readonly FakeAttribute[] Attributes;

		public FakeConstructor(FakeParameter[] parameters, FakeAttribute[] attributes = null) {
			Parameters = parameters;
			Attributes = attributes;
		}
	}
}