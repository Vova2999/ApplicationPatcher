using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonField : CommonBase<CommonField, FieldInfo, FieldDefinition>, IHasAttributes, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection.FieldType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }

		public CommonField(FieldInfo reflectionField, FieldDefinition monoCecilField) : base(reflectionField, monoCecilField) {
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
		}
	}
}