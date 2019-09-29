using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonField : ICommonMember<ICommonField, FieldDefinition, FieldInfo>, IHasAttributes, IHasType {
	}
}