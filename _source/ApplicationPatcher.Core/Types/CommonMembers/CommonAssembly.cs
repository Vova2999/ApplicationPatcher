using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using JetBrains.Annotations;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBeProtected.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonAssembly : CommonMemberBase<CommonAssembly, Assembly, AssemblyDefinition>, IHasAttributes, IHasTypes {
		public override string Name => GetOrCreate(() => MonoCecil.Name.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonType[] Types { get; private set; }

		internal virtual Dictionary<Type, CommonAttribute[]> TypeTypeToAttribute { get; private set; }
		Dictionary<Type, CommonAttribute[]> IHasAttributes.TypeTypeToAttribute => TypeTypeToAttribute;

		internal virtual Dictionary<string, CommonAttribute[]> TypeFullNameToAttribute { get; private set; }
		Dictionary<string, CommonAttribute[]> IHasAttributes.TypeFullNameToAttribute => TypeFullNameToAttribute;

		internal virtual Dictionary<Type, CommonType[]> TypeTypeToType { get; private set; }
		Dictionary<Type, CommonType[]> IHasTypes.TypeTypeToType => TypeTypeToType;

		internal virtual Dictionary<string, CommonType[]> TypeFullNameToType { get; private set; }
		Dictionary<string, CommonType[]> IHasTypes.TypeFullNameToType => TypeFullNameToType;

		[UsedImplicitly]
		public virtual CommonType[] TypesFromThisAssembly => GetOrCreate(() => Load().Types.WhereFrom(this).ToArray());

		public virtual Assembly[] ReferencedReflectionAssemblies { get; }
		public virtual AssemblyDefinition[] ReferencedMonoCecilAssemblies { get; }

		public readonly bool HaveSymbolStore;

		public CommonAssembly(Assembly mainReflectionAssembly,
							  Assembly[] referencedReflectionAssemblies,
							  AssemblyDefinition mainMonoCecilAssembly,
							  AssemblyDefinition[] referencedMonoCecilAssemblies,
							  bool haveSymbolStore) : base(mainReflectionAssembly, mainMonoCecilAssembly) {
			ReferencedReflectionAssemblies = referencedReflectionAssemblies;
			ReferencedMonoCecilAssemblies = referencedMonoCecilAssemblies;
			HaveSymbolStore = haveSymbolStore;
		}

		IHasAttributes ICommonMember<IHasAttributes>.Load() {
			return Load();
		}
		IHasTypes ICommonMember<IHasTypes>.Load() {
			return Load();
		}

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Types = CommonHelper.JoinTypes(
				new[] { Reflection }.Concat(ReferencedReflectionAssemblies).SelectMany(a => a.GetTypes()),
				new[] { MonoCecil }.Concat(ReferencedMonoCecilAssemblies).SelectMany(a => a.MainModule.Types));

			TypeTypeToAttribute = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttribute = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());

			TypeTypeToType = Types.GroupBy(type => type.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToType = Types.GroupBy(type => type.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}