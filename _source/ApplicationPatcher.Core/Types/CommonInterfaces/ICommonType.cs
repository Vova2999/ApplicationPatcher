using System;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonType : ICommonMember<ICommonType, TypeDefinition, Type>, IHasAttributes, IHasConstructors, IHasFields, IHasMethods, IHasProperties, IHasType {
	}
}