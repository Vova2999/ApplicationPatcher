using System;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAttribute : CommonBase<CommonAttribute, Attribute, CustomAttribute>, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection.GetType());
		public override string Name => GetOrCreate(() => MonoCecil.AttributeType.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.AttributeType.FullName);

		public CommonAttribute(Attribute reflectionAttribute, CustomAttribute monoCecilAttribute) : base(reflectionAttribute, monoCecilAttribute) {
		}
	}
}