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
		// todo: remove GetOrCreate?
		public override string Name => GetOrCreate(() => MonoCecil.Name.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		public ICommonType[] Types { get; private set; }
		public ICommonAttribute[] Attributes { get; private set; }
		public IDictionary<Type, ICommonType[]> TypeTypeToTypes { get; private set; }
		public IDictionary<string, ICommonType[]> TypeFullNameToTypes { get; private set; }
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes { get; private set; }
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes { get; private set; }

		public ICommonType[] TypesFromThisAssembly => GetOrCreate(() => Load().Types.WhereFrom(this).ToArray());

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
			Types = CommonHelper.JoinTypes(
				new[] { Reflection }.Concat(ReferencedReflectionAssemblies).SelectMany(a => a.GetTypes()),
				new[] { MonoCecil }.Concat(ReferencedMonoCecilAssemblies).SelectMany(a => a.MainModule.Types));

			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);

			TypeTypeToTypes = Types.GroupBy(type => type.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToTypes = Types.GroupBy(type => type.FullName).ToDictionary(group => group.Key, group => group.ToArray());

			TypeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}