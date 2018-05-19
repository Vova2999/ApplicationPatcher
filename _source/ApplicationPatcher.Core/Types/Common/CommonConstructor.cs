using System;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonConstructor : CommonBase<CommonConstructor>, IHasAttributes, IHasParameters {
		public override string Name => GetOrCreate(() => MonoCecilConstructor.Name);
		public override string FullName => GetOrCreate(() => MonoCecilConstructor.FullName);
		public CommonAttribute[] Attributes { get; private set; }
		public CommonParameter[] Parameters { get; private set; }

		[UsedImplicitly]
		public Type[] ParameterTypes => GetOrCreate(() => ReflectionConstructor.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public readonly ConstructorInfo ReflectionConstructor;
		public readonly MethodDefinition MonoCecilConstructor;

		public CommonConstructor(ConstructorInfo reflectionConstructor, MethodDefinition monoCecilConstructor) {
			ReflectionConstructor = reflectionConstructor;
			MonoCecilConstructor = monoCecilConstructor;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(ReflectionConstructor.GetCustomAttributesData(), MonoCecilConstructor.CustomAttributes);
			Parameters = CommonHelper.JoinParameters(ReflectionConstructor.GetParameters(), MonoCecilConstructor.Parameters);
		}
	}
}