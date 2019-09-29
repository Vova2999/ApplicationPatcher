using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonAssembly : ICommonMember<ICommonAssembly, AssemblyDefinition, Assembly>, IHasAttributes, IHasTypes {
		ICommonType[] TypesFromThisAssembly { get; }

		bool HaveSymbolStore { get; }
		Assembly[] ReferencedReflectionAssemblies { get; }
		AssemblyDefinition[] ReferencedMonoCecilAssemblies { get; }
	}
}