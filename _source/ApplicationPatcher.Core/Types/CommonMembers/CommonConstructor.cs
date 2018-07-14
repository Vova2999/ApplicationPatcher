using System;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using JetBrains.Annotations;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonConstructor : CommonMemberBase<CommonConstructor, ConstructorInfo, MethodDefinition>, IHasAttributes, IHasParameters {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonParameter[] Parameters { get; private set; }

		[UsedImplicitly]
		public virtual Type[] ParameterTypes => GetOrCreate(() => Reflection.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public CommonConstructor(ConstructorInfo reflectionConstructor, MethodDefinition monoCecilConstructor) : base(reflectionConstructor, monoCecilConstructor) {
		}

		IHasAttributes ICommonMember<IHasAttributes>.Load() {
			return Load();
		}
		IHasParameters ICommonMember<IHasParameters>.Load() {
			return Load();
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Parameters = CommonHelper.JoinParameters(Reflection.GetParameters(), MonoCecil.Parameters);
		}
	}
}