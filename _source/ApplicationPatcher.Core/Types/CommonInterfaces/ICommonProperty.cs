using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonProperty : ICommonMember<ICommonProperty, PropertyDefinition, PropertyInfo>, IHasAttributes, IHasType {
		ICommonMethod GetMethod { get; }
		ICommonMethod SetMethod { get; }
	}
}