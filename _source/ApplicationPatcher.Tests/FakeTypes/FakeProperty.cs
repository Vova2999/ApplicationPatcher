namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeProperty {
		public string Name { get; set; }
		public FakeMethod GetMethod { get; set; }
		public FakeMethod SetMethod { get; set; }
		public FakeType PropertyType { get; set; }
		public FakeAttribute[] Attributes { get; set; }
	}
}