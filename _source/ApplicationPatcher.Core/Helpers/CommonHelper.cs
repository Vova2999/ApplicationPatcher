using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.CommonMembers;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Helpers {
	internal static class CommonHelper {
		internal static CommonAttribute[] JoinAttributes(IEnumerable<CustomAttributeData> reflectionAttributes, IEnumerable<CustomAttribute> monoCecilAttributes) {
			return Join(
				reflectionAttributes,
				monoCecilAttributes,
				reflectionAttribute => $"{reflectionAttribute.AttributeType.FullName}({reflectionAttribute.ConstructorArguments.Select(argument => $"{argument.ArgumentType.FullName}: {argument.Value}").JoinToString(", ")})",
				monoCecilAttribute => $"{monoCecilAttribute.AttributeType.FullName}({monoCecilAttribute.ConstructorArguments.Select(argument => $"{argument.Type.FullName}: {argument.Value}").JoinToString(", ")})",
				attributeFullName => true,
				(reflectionAttribute, monoCecilAttribute) => {
					var attribute = (Attribute)reflectionAttribute.Constructor.Invoke(reflectionAttribute.ConstructorArguments.Select(argument => argument.Value).ToArray());

					if (reflectionAttribute.NamedArguments == null)
						return new CommonAttribute(attribute, monoCecilAttribute);

					foreach (var namedArgument in reflectionAttribute.NamedArguments) {
						switch (namedArgument.MemberInfo) {
							case FieldInfo fieldInfo:
								fieldInfo.SetValue(attribute, namedArgument.TypedValue.Value);
								break;

							case PropertyInfo propertyInfo:
								propertyInfo.SetValue(attribute, namedArgument.TypedValue.Value, null);
								break;
						}
					}

					return new CommonAttribute(attribute, monoCecilAttribute);
				});
		}

		internal static CommonConstructor[] JoinConstructors(IEnumerable<ConstructorInfo> reflectionConstructors, IEnumerable<MethodDefinition> monoCecilConstructors) {
			return Join(
				reflectionConstructors,
				monoCecilConstructors,
				reflectionConstructor => $"{reflectionConstructor.Name}({reflectionConstructor.GetParameters().Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				monoCecilConstructor => $"{monoCecilConstructor.Name}({monoCecilConstructor.Parameters.Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				constructorName => constructorName.StartsWith(".ctor"),
				(reflectionConstructor, monoCecilConstructor) => new CommonConstructor(reflectionConstructor, monoCecilConstructor));
		}

		internal static CommonField[] JoinFields(IEnumerable<FieldInfo> reflectionFields, IEnumerable<FieldDefinition> monoCecilFields) {
			return Join(
				reflectionFields,
				monoCecilFields,
				reflectionField => reflectionField.Name,
				monoCecilField => monoCecilField.Name,
				fieldName => true,
				(reflectionField, monoCecilField) => new CommonField(reflectionField, monoCecilField));
		}

		internal static CommonMethod[] JoinMethods(IEnumerable<MethodInfo> reflectionMethods, IEnumerable<MethodDefinition> monoCecilMethods) {
			return Join(
				reflectionMethods,
				monoCecilMethods,
				reflectionMethod => $"{reflectionMethod.Name}({reflectionMethod.GetParameters().Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				monoCecilMethod => $"{monoCecilMethod.Name}({monoCecilMethod.Parameters.Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				methodName => !methodName.StartsWith("get_") && !methodName.StartsWith("set_"),
				(reflectionMethod, monoCecilMethod) => new CommonMethod(reflectionMethod, monoCecilMethod));
		}

		internal static CommonParameter[] JoinParameters(IEnumerable<ParameterInfo> reflectionParameters, IEnumerable<ParameterDefinition> monoCecilParameters) {
			return Join(
				reflectionParameters,
				monoCecilParameters,
				reflectionParameter => $"{reflectionParameter.Name}:{reflectionParameter.ParameterType.Name}",
				monoCecilParameter => $"{monoCecilParameter.Name}:{monoCecilParameter.ParameterType.Name}",
				parameterName => true,
				(reflectionParameter, monoCecilParameter) => new CommonParameter(reflectionParameter, monoCecilParameter));
		}

		internal static CommonProperty[] JoinProperties(IEnumerable<PropertyInfo> reflectionProperties, IEnumerable<PropertyDefinition> monoCecilProperties) {
			return Join(
				reflectionProperties,
				monoCecilProperties,
				reflectionProperty => reflectionProperty.Name,
				monoCecilProperty => monoCecilProperty.Name,
				propertyName => true,
				(reflectionProperty, monoCecilProperty) => new CommonProperty(reflectionProperty, monoCecilProperty));
		}

		internal static CommonType[] JoinTypes(IEnumerable<Type> reflectionTypes, IEnumerable<TypeDefinition> monoCecilTypes) {
			return Join(
				reflectionTypes,
				monoCecilTypes,
				reflectionType => reflectionType.AssemblyQualifiedName,
				monoCecilType => $"{monoCecilType.FullName}, {monoCecilType.Module.Assembly.FullName}",
				typeFullName => true,
				(reflectionType, monoCecilType) => new CommonType(reflectionType, monoCecilType));
		}

		private static TCommon[] Join<TCommon, TReflection, TMonoCecil>(IEnumerable<TReflection> reflections,
																		IEnumerable<TMonoCecil> monoCecils,
																		Func<TReflection, string> getReflectionName,
																		Func<TMonoCecil, string> getMonoCecilName,
																		Func<string, bool> isSatisfyForSelection,
																		Func<TReflection, TMonoCecil, TCommon> createCommon) {
			var reflectionAttributesDictionary = reflections.ToDictionary(getReflectionName);
			var monoCecilAttributesDictionary = monoCecils.ToDictionary(getMonoCecilName);

			return reflectionAttributesDictionary.Keys
				.Intersect(monoCecilAttributesDictionary.Keys)
				.Where(isSatisfyForSelection)
				.Select(key => createCommon(reflectionAttributesDictionary[key], monoCecilAttributesDictionary[key]))
				.ToArray();
		}
	}
}