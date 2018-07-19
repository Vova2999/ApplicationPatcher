using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace ApplicationPatcher.Core.Extensions {
	public static class MethodReferenceExtensions {
		public static MethodReference MakeHostInstanceGeneric(this MethodReference methodReference, params TypeReference[] typeReferences) {
			var reference = new MethodReference(
				methodReference.Name,
				methodReference.ReturnType,
				methodReference.DeclaringType.MakeGenericInstanceType(typeReferences)) {
				HasThis = methodReference.HasThis,
				ExplicitThis = methodReference.ExplicitThis,
				CallingConvention = methodReference.CallingConvention
			};

			foreach (var parameter in methodReference.Parameters)
				reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

			foreach (var genericParam in methodReference.GenericParameters)
				reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));

			return reference;
		}
	}
}