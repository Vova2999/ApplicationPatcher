﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using JetBrains.Annotations;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonProperty : CommonMemberBase<CommonProperty, PropertyInfo, PropertyDefinition>, IHasAttributes, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection.PropertyType);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }

		internal virtual Dictionary<Type, CommonAttribute[]> TypeTypeToAttribute { get; private set; }
		Dictionary<Type, CommonAttribute[]> IHasAttributes.TypeTypeToAttribute => TypeTypeToAttribute;

		internal virtual Dictionary<string, CommonAttribute[]> TypeFullNameToAttribute { get; private set; }
		Dictionary<string, CommonAttribute[]> IHasAttributes.TypeFullNameToAttribute => TypeFullNameToAttribute;

		[UsedImplicitly]
		public virtual CommonMethod GetMethod => GetOrCreate(() => Reflection.GetMethod == null ? null : new CommonMethod(Reflection.GetMethod, MonoCecil.GetMethod));

		[UsedImplicitly]
		public virtual CommonMethod SetMethod => GetOrCreate(() => Reflection.SetMethod == null ? null : new CommonMethod(Reflection.SetMethod, MonoCecil.SetMethod));

		public CommonProperty(PropertyInfo reflectionProperty, PropertyDefinition monoCecilProperty) : base(reflectionProperty, monoCecilProperty) {
		}

		IHasAttributes ICommonMember<IHasAttributes>.Load() {
			return Load();
		}
		IHasType ICommonMember<IHasType>.Load() {
			return Load();
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			TypeTypeToAttribute = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttribute = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}