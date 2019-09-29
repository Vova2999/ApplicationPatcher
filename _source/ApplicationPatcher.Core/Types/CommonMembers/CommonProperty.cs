using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonProperty : CommonMember<ICommonProperty, PropertyDefinition, PropertyInfo>, ICommonProperty {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		public Type Type => GetOrCreate(() => Reflection.PropertyType);

		private ICommonAttribute[] attributes;
		public ICommonAttribute[] Attributes => attributes.CheckLoaded();

		private IDictionary<Type, ICommonAttribute[]> typeTypeToAttributes;
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes => typeTypeToAttributes.CheckLoaded();

		private IDictionary<string, ICommonAttribute[]> typeFullNameToAttributes;
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes => typeFullNameToAttributes.CheckLoaded();

		public ICommonMethod GetMethod => GetOrCreate(() => Reflection.GetMethod == null ? null : new CommonMethod(MonoCecil.GetMethod, Reflection.GetMethod));
		public ICommonMethod SetMethod => GetOrCreate(() => Reflection.SetMethod == null ? null : new CommonMethod(MonoCecil.SetMethod, Reflection.SetMethod));

		public CommonProperty(PropertyDefinition monoCecilProperty, PropertyInfo reflectionProperty) : base(monoCecilProperty, reflectionProperty) {
		}

		protected override void LoadInternal() {
			attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			typeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}