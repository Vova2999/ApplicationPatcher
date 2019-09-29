using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonConstructor : CommonMember<ICommonConstructor, MethodDefinition, ConstructorInfo>, ICommonConstructor {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		public ICommonAttribute[] Attributes { get; private set; }
		public ICommonParameter[] Parameters { get; private set; }
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes { get; private set; }
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes { get; private set; }

		public Type[] ParameterTypes => GetOrCreate(() => Reflection.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public CommonConstructor(MethodDefinition monoCecilConstructor, ConstructorInfo reflectionConstructor) : base(monoCecilConstructor, reflectionConstructor) {
		}

		protected override void LoadInternal() {
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Parameters = CommonHelper.JoinParameters(Reflection.GetParameters(), MonoCecil.Parameters);

			TypeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}