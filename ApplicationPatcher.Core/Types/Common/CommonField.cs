using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonField : CommonBase<CommonField>, IHasAttributes {
		public CommonAttribute[] Attributes { get; private set; }
		public readonly FieldInfo ReflectionField;
		public readonly FieldDefinition MonoCecilField;

		public CommonField(FieldInfo reflectionField, FieldDefinition monoCecilField) {
			ReflectionField = reflectionField;
			MonoCecilField = monoCecilField;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(ReflectionField.GetCustomAttributes(), MonoCecilField.CustomAttributes);
		}
	}
}