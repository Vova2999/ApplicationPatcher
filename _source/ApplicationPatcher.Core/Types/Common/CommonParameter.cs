using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonParameter : CommonBase<CommonParameter>, IHasType {
		public Type Type => GetOrCreate(() => ReflectionParameter.ParameterType);
		public override string Name => GetOrCreate(() => MonoCecilParameter.Name);
		public override string FullName => GetOrCreate(() => MonoCecilParameter.Name);

		[UsedImplicitly]
		public readonly ParameterInfo ReflectionParameter;
		[UsedImplicitly]
		public readonly ParameterDefinition MonoCecilParameter;

		public CommonParameter(ParameterInfo reflectionParameter, ParameterDefinition monoCecilParameter) {
			ReflectionParameter = reflectionParameter;
			MonoCecilParameter = monoCecilParameter;
		}
	}
}