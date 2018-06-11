using System;

namespace ApplicationPatcher.Tests.FakeTypes {
	public class FakeParameter {
		public readonly string Name;
		public readonly FakeType ParameterType;

		public FakeParameter(string name, Type parameterType) : this(name, new FakeType(parameterType)) {
		}
		public FakeParameter(string name, FakeType parameterType) {
			Name = name;
			ParameterType = parameterType;
		}
	}
}