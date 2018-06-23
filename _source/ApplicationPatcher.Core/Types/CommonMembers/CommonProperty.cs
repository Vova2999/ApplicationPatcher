using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonProperty : CommonMemberBase<CommonProperty, PropertyInfo, PropertyDefinition>, IHasAttributes, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection.PropertyType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }

		public CommonProperty(PropertyInfo reflectionProperty, PropertyDefinition monoCecilProperty) : base(reflectionProperty, monoCecilProperty) {
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
		}
	}
}