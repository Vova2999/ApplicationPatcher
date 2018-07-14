using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.Interfaces;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonParameter : CommonMemberBase<CommonParameter, ParameterInfo, ParameterDefinition>, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection.ParameterType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.Name);

		public CommonParameter(ParameterInfo reflectionParameter, ParameterDefinition monoCecilParameter) : base(reflectionParameter, monoCecilParameter) {
		}

		IHasType ICommonMember<IHasType>.Load() {
			return Load();
		}
	}
}