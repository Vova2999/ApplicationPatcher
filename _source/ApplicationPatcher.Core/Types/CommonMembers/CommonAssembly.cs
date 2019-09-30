using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	[PublicAPI]
	public class CommonAssembly : CommonMember<ICommonAssembly, AssemblyDefinition, Assembly>, ICommonAssembly {
		public override string Name => GetOrCreate(() => MonoCecil.Name.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		private ICommonType[] types;
		public ICommonType[] Types => types.CheckLoaded();

		private ICommonAttribute[] attributes;
		public ICommonAttribute[] Attributes => attributes.CheckLoaded();

		private IDictionary<Type, ICommonType[]> typeTypeToTypes;
		public IDictionary<Type, ICommonType[]> TypeTypeToTypes => typeTypeToTypes.CheckLoaded();

		private IDictionary<string, ICommonType[]> typeFullNameToTypes;
		public IDictionary<string, ICommonType[]> TypeFullNameToTypes => typeFullNameToTypes.CheckLoaded();

		private IDictionary<Type, ICommonAttribute[]> typeTypeToAttributes;
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes => typeTypeToAttributes.CheckLoaded();

		private IDictionary<string, ICommonAttribute[]> typeFullNameToAttributes;
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes => typeFullNameToAttributes.CheckLoaded();

		public ICommonType[] TypesFromThisAssembly => GetOrCreate(() => Types.WhereFrom(this).ToArray());

		public bool HaveSymbolStore { get; }
		public Assembly[] ReferencedReflectionAssemblies { get; }
		public AssemblyDefinition[] ReferencedMonoCecilAssemblies { get; }

		public CommonAssembly(AssemblyDefinition mainMonoCecilAssembly,
							  AssemblyDefinition[] referencedMonoCecilAssemblies,
							  Assembly mainReflectionAssembly,
							  Assembly[] referencedReflectionAssemblies,
							  bool haveSymbolStore) : base(mainMonoCecilAssembly, mainReflectionAssembly) {
			ReferencedMonoCecilAssemblies = referencedMonoCecilAssemblies;
			ReferencedReflectionAssemblies = referencedReflectionAssemblies;
			HaveSymbolStore = haveSymbolStore;
		}

		protected override void LoadInternal() {
			types = CommonHelper.JoinTypes(
				new[] { Reflection }.Concat(ReferencedReflectionAssemblies).SelectMany(a => a.GetTypes()),
				new[] { MonoCecil }.Concat(ReferencedMonoCecilAssemblies).SelectMany(a => a.MainModule.Types));

			attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			typeTypeToTypes = Types.GroupBy(type => type.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToTypes = Types.GroupBy(type => type.FullName).ToDictionary(group => group.Key, group => group.ToArray());

			typeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}

		protected override void LoadInDepth(int depth) {
			Types.ForEach(type => type.Load(depth));
			Attributes.ForEach(attribute => attribute.Load(depth));
		}
	}
}