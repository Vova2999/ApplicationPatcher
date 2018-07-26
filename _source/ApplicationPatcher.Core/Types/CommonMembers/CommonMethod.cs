using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using JetBrains.Annotations;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonMethod : CommonMemberBase<CommonMethod, MethodInfo, MethodDefinition>, IHasAttributes, IHasParameters {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonParameter[] Parameters { get; private set; }

		internal virtual Dictionary<Type, CommonAttribute[]> TypeTypeToAttribute { get; private set; }
		Dictionary<Type, CommonAttribute[]> IHasAttributes.TypeTypeToAttribute => TypeTypeToAttribute;

		internal virtual Dictionary<string, CommonAttribute[]> TypeFullNameToAttribute { get; private set; }
		Dictionary<string, CommonAttribute[]> IHasAttributes.TypeFullNameToAttribute => TypeFullNameToAttribute;

		[UsedImplicitly]
		public virtual Type ReturnType => GetOrCreate(() => Reflection.ReturnType);

		[UsedImplicitly]
		public virtual Type[] ParameterTypes => GetOrCreate(() => Reflection.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public CommonMethod(MethodInfo reflectionMethod, MethodDefinition monoCecilMethod) : base(reflectionMethod, monoCecilMethod) {
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

			TypeTypeToAttribute = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttribute = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}