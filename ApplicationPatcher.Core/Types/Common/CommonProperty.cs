using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonProperty : CommonBase<CommonProperty>, IHasAttributes {
		public CommonAttribute[] Attributes { get; private set; }
		public readonly PropertyInfo ReflectionProperty;
		public readonly PropertyDefinition MonoCecilProperty;

		public CommonProperty(PropertyInfo reflectionProperty, PropertyDefinition monoCecilProperty) {
			ReflectionProperty = reflectionProperty;
			MonoCecilProperty = monoCecilProperty;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(ReflectionProperty.GetCustomAttributes(), MonoCecilProperty.CustomAttributes);
		}
	}
}