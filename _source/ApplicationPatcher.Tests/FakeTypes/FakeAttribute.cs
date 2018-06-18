using System;

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeAttribute {
		public readonly Type AttributeType;
		public readonly Attribute AttributeInstance;

		public FakeAttribute(Attribute attributeInstance) {
			AttributeType = attributeInstance.GetType();
			AttributeInstance = attributeInstance;
		}
	}
}