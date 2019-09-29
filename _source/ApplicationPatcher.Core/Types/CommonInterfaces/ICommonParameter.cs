using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonParameter : ICommonMember<ICommonParameter, ParameterDefinition, ParameterInfo>, IHasType {
	}
}