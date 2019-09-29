using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonMethod : ICommonMember<ICommonMethod, MethodDefinition, MethodInfo>, IHasAttributes, IHasParameters {
		Type ReturnType { get; }
		Type[] ParameterTypes { get; }
	}
}