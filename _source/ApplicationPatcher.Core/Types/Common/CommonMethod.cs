using System;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonMethod : CommonBase<CommonMethod, MethodInfo, MethodDefinition>, IHasAttributes, IHasParameters {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonParameter[] Parameters { get; private set; }

		[UsedImplicitly]
		public virtual Type[] ParameterTypes => GetOrCreate(() => Reflection.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public CommonMethod(MethodInfo reflectionMethod, MethodDefinition monoCecilMethod) : base(reflectionMethod, monoCecilMethod) {
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Parameters = CommonHelper.JoinParameters(Reflection.GetParameters(), MonoCecil.Parameters);
		}
	}
}