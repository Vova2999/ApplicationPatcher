using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	internal class CommonParameter : CommonMember<ICommonParameter, ParameterDefinition, ParameterInfo>, ICommonParameter {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.Name);

		public Type Type => GetOrCreate(() => Reflection.ParameterType);

		private ICommonAttribute[] attributes;
		public ICommonAttribute[] Attributes => attributes.CheckLoaded();

		private IDictionary<Type, ICommonAttribute[]> typeTypeToAttributes;
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes => typeTypeToAttributes.CheckLoaded();

		private IDictionary<string, ICommonAttribute[]> typeFullNameToAttributes;
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes => typeFullNameToAttributes.CheckLoaded();

		public CommonParameter(ParameterDefinition monoCecilParameter, ParameterInfo reflectionParameter) : base(monoCecilParameter, reflectionParameter) {
		}

		protected override void LoadInternal() {
			attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			typeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}

		protected override void LoadInDepth(int depth) {
			Attributes.ForEach(attribute => attribute.Load(depth));
		}
	}
}