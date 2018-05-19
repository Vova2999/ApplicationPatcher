using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAssembly : CommonBase<CommonAssembly>, IHasAttributes, IHasTypes {
		public override string Name => GetOrCreate(() => MainMonoCecilAssembly.FullName);
		public override string FullName => GetOrCreate(() => MainMonoCecilAssembly.FullName);
		public CommonAttribute[] Attributes { get; private set; }
		public CommonType[] Types { get; private set; }

		[UsedImplicitly]
		public CommonType[] TypesFromThisAssembly => GetOrCreate(() => Types.WhereFrom(this).ToArray());

		public readonly bool HaveSymbolStore;

		[UsedImplicitly]
		public readonly Assembly MainReflectionAssembly;

		[UsedImplicitly]
		public readonly Assembly[] ReferencedReflectionAssemblies;

		[UsedImplicitly]
		public virtual AssemblyDefinition MainMonoCecilAssembly { get; }

		[UsedImplicitly]
		public readonly AssemblyDefinition[] ReferencedMonoCecilAssemblies;

		public CommonAssembly(Assembly mainReflectionAssembly,
							  Assembly[] referencedReflectionAssemblies,
							  AssemblyDefinition mainMonoCecilAssembly,
							  AssemblyDefinition[] referencedMonoCecilAssemblies,
							  bool haveSymbolStore) {
			MainReflectionAssembly = mainReflectionAssembly;
			ReferencedReflectionAssemblies = referencedReflectionAssemblies;
			MainMonoCecilAssembly = mainMonoCecilAssembly;
			ReferencedMonoCecilAssemblies = referencedMonoCecilAssemblies;
			HaveSymbolStore = haveSymbolStore;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Attributes = CommonHelper.JoinAttributes(MainReflectionAssembly.GetCustomAttributesData(), MainMonoCecilAssembly.CustomAttributes);
			Types = CommonHelper.JoinTypes(
				new[] { MainReflectionAssembly }.Concat(ReferencedReflectionAssemblies).SelectMany(a => a.GetTypes()),
				new[] { MainMonoCecilAssembly }.Concat(ReferencedMonoCecilAssemblies).SelectMany(a => a.MainModule.Types));
		}
	}
}