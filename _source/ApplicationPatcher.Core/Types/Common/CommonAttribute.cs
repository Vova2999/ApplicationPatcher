using System;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAttribute : CommonBase<CommonAttribute> {
		public override string Name => GetOrCreate(() => MonoCecilAttribute.AttributeType.Name);
		public override string FullName => GetOrCreate(() => MonoCecilAttribute.AttributeType.FullName);

		[UsedImplicitly]
		public readonly Attribute ReflectionAttribute;
		[UsedImplicitly]
		public readonly CustomAttribute MonoCecilAttribute;

		public CommonAttribute(Attribute reflectionAttribute, CustomAttribute monoCecilAttribute) {
			ReflectionAttribute = reflectionAttribute;
			MonoCecilAttribute = monoCecilAttribute;
		}
	}
}