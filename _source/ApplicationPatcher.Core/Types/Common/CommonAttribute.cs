using System;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAttribute : CommonBase<CommonAttribute>, IHasType {
		public Type Type => GetOrCreate(() => ReflectionAttribute.GetType());
		public override string Name => GetOrCreate(() => MonoCecilAttribute.AttributeType.Name);
		public override string FullName => GetOrCreate(() => MonoCecilAttribute.AttributeType.FullName);

		public readonly Attribute ReflectionAttribute;
		public readonly CustomAttribute MonoCecilAttribute;

		public CommonAttribute(Attribute reflectionAttribute, CustomAttribute monoCecilAttribute) {
			ReflectionAttribute = reflectionAttribute;
			MonoCecilAttribute = monoCecilAttribute;
		}
	}
}