using System;
using JetBrains.Annotations;

namespace ApplicationPatcher.Tests.FakeTypes {
	[PublicAPI]
	public class FakeAttribute {
		public readonly Type AttributeType;
		public readonly Attribute AttributeInstance;

		public FakeAttribute(Attribute attributeInstance) {
			AttributeType = attributeInstance.GetType();
			AttributeInstance = attributeInstance;
		}
	}
}