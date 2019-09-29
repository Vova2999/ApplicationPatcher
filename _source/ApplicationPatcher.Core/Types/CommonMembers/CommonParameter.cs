using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonParameter : CommonMember<ICommonParameter, ParameterDefinition, ParameterInfo>, ICommonParameter {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.Name);

		public Type Type => GetOrCreate(() => Reflection.ParameterType);
		public ICommonAttribute[] Attributes { get; private set; }
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes { get; private set; }
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes { get; private set; }

		public CommonParameter(ParameterDefinition monoCecilParameter, ParameterInfo reflectionParameter) : base(monoCecilParameter, reflectionParameter) {
		}

		protected override void LoadInternal() {
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			TypeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}