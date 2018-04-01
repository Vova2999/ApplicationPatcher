﻿using System.Reflection;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonParameter : CommonBase<CommonParameter> {
		public readonly ParameterInfo ReflectionParameter;
		public readonly ParameterDefinition MonoCecilParameter;

		public CommonParameter(ParameterInfo reflectionParameter, ParameterDefinition monoCecilParameter) {
			ReflectionParameter = reflectionParameter;
			MonoCecilParameter = monoCecilParameter;
		}
	}
}