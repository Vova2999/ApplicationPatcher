﻿using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonParameter : CommonBase<CommonParameter, ParameterInfo, ParameterDefinition>, IHasType {
		public Type Type => GetOrCreate(() => Reflection.ParameterType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.Name);

		public CommonParameter(ParameterInfo reflectionParameter, ParameterDefinition monoCecilParameter) : base(reflectionParameter, monoCecilParameter) {
		}
	}
}