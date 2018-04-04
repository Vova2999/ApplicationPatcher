using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonProperty : CommonBase<CommonProperty>, IHasAttributes {
		public override string Name => GetOrCreate(() => MonoCecilProperty.Name);
		public override string FullName => GetOrCreate(() => MonoCecilProperty.FullName);
		public CommonAttribute[] Attributes { get; private set; }

		[UsedImplicitly]
		public readonly PropertyInfo ReflectionProperty;
		[UsedImplicitly]
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