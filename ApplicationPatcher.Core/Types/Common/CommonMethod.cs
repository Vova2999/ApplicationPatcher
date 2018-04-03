using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonMethod : CommonBase<CommonMethod>, IHasAttributes {
		public override string Name => GetOrCreate(() => MonoCecilMethod.Name);
		public override string FullName => GetOrCreate(() => MonoCecilMethod.FullName);
		public CommonAttribute[] Attributes { get; private set; }
		public readonly MethodInfo ReflectionMethod;
		public readonly MethodDefinition MonoCecilMethod;

		public CommonMethod(MethodInfo reflectionMethod, MethodDefinition monoCecilMethod) {
			ReflectionMethod = reflectionMethod;
			MonoCecilMethod = monoCecilMethod;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(ReflectionMethod.GetCustomAttributes(), MonoCecilMethod.CustomAttributes);
		}
	}
}