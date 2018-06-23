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
		public override string Name => GetOrCreate(() => MonoCecil.FullName);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonType[] Types { get; private set; }

		[UsedImplicitly]
		public CommonType[] TypesFromThisAssembly => GetOrCreate(() => Types.CheckLoaded().WhereFrom(this).ToArray());

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

		internal override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Types = CommonHelper.JoinTypes(
				new[] { Reflection }.Concat(ReferencedReflectionAssemblies).SelectMany(a => a.GetTypes()),
				new[] { MonoCecil }.Concat(ReferencedMonoCecilAssemblies).SelectMany(a => a.MainModule.Types));
		}
	}
}