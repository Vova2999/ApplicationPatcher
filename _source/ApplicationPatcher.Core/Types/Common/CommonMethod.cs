using System;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonMethod : CommonBase<CommonMethod>, IHasAttributes, IHasParameters {
		public override string Name => GetOrCreate(() => MonoCecilMethod.Name);
		public override string FullName => GetOrCreate(() => MonoCecilMethod.FullName);
		public CommonAttribute[] Attributes { get; private set; }
		public CommonParameter[] Parameters { get; private set; }

		[UsedImplicitly]
		public Type[] ParameterTypes => GetOrCreate(() => ReflectionMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public readonly MethodInfo ReflectionMethod;
		public readonly MethodDefinition MonoCecilMethod;

		public CommonMethod(MethodInfo reflectionMethod, MethodDefinition monoCecilMethod) {
			ReflectionMethod = reflectionMethod;
			MonoCecilMethod = monoCecilMethod;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(ReflectionMethod.GetCustomAttributesData(), MonoCecilMethod.CustomAttributes);
			Parameters = CommonHelper.JoinParameters(ReflectionMethod.GetParameters(), MonoCecilMethod.Parameters);
		}
	}
}