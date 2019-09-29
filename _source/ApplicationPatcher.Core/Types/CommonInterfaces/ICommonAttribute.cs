using System;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonAttribute : ICommonMember<ICommonAttribute, CustomAttribute, Attribute>, IHasType {
	}
}