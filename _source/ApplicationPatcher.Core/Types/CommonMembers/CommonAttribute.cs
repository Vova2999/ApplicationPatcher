using System;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonAttribute : CommonMember<ICommonAttribute, CustomAttribute, Attribute>, ICommonAttribute {
		public override string Name => GetOrCreate(() => MonoCecil.AttributeType.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.AttributeType.FullName);

		public Type Type => GetOrCreate(() => Reflection.GetType());

		public CommonAttribute(CustomAttribute monoCecilAttribute, Attribute reflectionAttribute) : base(monoCecilAttribute, reflectionAttribute) {
		}

		protected override void LoadInternal() {
		}
	}
}