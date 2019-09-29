using System;
using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonInterfaces {
	[PublicAPI]
	public interface ICommonConstructor : ICommonMember<ICommonConstructor, MethodDefinition, ConstructorInfo>, IHasAttributes, IHasParameters {
		Type[] ParameterTypes { get; }
	}
}