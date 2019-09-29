using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonParameter : CommonMember<ICommonParameter, ParameterDefinition, ParameterInfo>, ICommonParameter {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.Name);

		public Type Type => GetOrCreate(() => Reflection.ParameterType);

		public CommonParameter(ParameterDefinition monoCecilParameter, ParameterInfo reflectionParameter) : base(monoCecilParameter, reflectionParameter) {
		}

		protected override void LoadInternal() {
		}
	}
}