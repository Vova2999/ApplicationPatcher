using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonProperty : CommonBase<CommonProperty, PropertyInfo, PropertyDefinition>, IHasAttributes, IHasType {
		public Type Type => GetOrCreate(() => Reflection.PropertyType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public CommonAttribute[] Attributes { get; private set; }

		public CommonProperty(PropertyInfo reflectionProperty, PropertyDefinition monoCecilProperty) : base(reflectionProperty, monoCecilProperty) {
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
		}
	}
}