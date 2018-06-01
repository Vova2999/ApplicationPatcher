using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonField : CommonBase<CommonField, FieldInfo, FieldDefinition>, IHasAttributes, IHasType {
		public Type Type => GetOrCreate(() => Reflection.FieldType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public CommonAttribute[] Attributes { get; private set; }

		public CommonField(FieldInfo reflectionField, FieldDefinition monoCecilField) : base(reflectionField, monoCecilField) {
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
		}
	}
}