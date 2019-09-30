using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	internal class CommonMethod : CommonMember<ICommonMethod, MethodDefinition, MethodInfo>, ICommonMethod {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		private ICommonAttribute[] attributes;
		public ICommonAttribute[] Attributes => attributes.CheckLoaded();

		private ICommonParameter[] parameters;
		public ICommonParameter[] Parameters => parameters.CheckLoaded();

		private IDictionary<Type, ICommonAttribute[]> typeTypeToAttributes;
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes => typeTypeToAttributes.CheckLoaded();

		private IDictionary<string, ICommonAttribute[]> typeFullNameToAttributes;
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes => typeFullNameToAttributes.CheckLoaded();

		public Type ReturnType => GetOrCreate(() => Reflection.ReturnType);
		public Type[] ParameterTypes => GetOrCreate(() => Reflection.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

		public CommonMethod(MethodDefinition monoCecilMethod, MethodInfo reflectionMethod) : base(monoCecilMethod, reflectionMethod) {
		}

		protected override void LoadInternal() {
			attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			parameters = CommonHelper.JoinParameters(Reflection.GetParameters(), MonoCecil.Parameters);

			typeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}

		protected override void LoadInDepth(int depth) {
			Attributes.ForEach(attribute => attribute.Load(depth));
			Parameters.ForEach(parameter => parameter.Load(depth));
		}
	}
}