using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using ApplicationPatcher.Core.Types.CommonMembers;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Helpers {
	internal static class CommonHelper {
		internal static ICommonAttribute[] JoinAttributes(IEnumerable<CustomAttributeData> reflectionAttributes, IEnumerable<CustomAttribute> monoCecilAttributes) {
			return Join(
				monoCecilAttributes,
				reflectionAttributes,
				monoCecilAttribute => $"{monoCecilAttribute.AttributeType.FullName}({monoCecilAttribute.ConstructorArguments.Select(argument => $"{argument.Type.FullName}: {argument.Value}").JoinToString(", ")})",
				reflectionAttribute => $"{reflectionAttribute.AttributeType.FullName}({reflectionAttribute.ConstructorArguments.Select(argument => $"{argument.ArgumentType.FullName}: {argument.Value}").JoinToString(", ")})",
				attributeFullName => true,
				(reflectionAttribute, monoCecilAttribute) => {
					var attribute = (Attribute)reflectionAttribute.Constructor.Invoke(reflectionAttribute.ConstructorArguments.Select(argument => argument.Value).ToArray());

					if (reflectionAttribute.NamedArguments == null)
						return new CommonAttribute(monoCecilAttribute, attribute);

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

					return (ICommonAttribute)new CommonAttribute(monoCecilAttribute, attribute);
				});
		}

		internal static ICommonConstructor[] JoinConstructors(IEnumerable<ConstructorInfo> reflectionConstructors, IEnumerable<MethodDefinition> monoCecilConstructors) {
			return Join(
				monoCecilConstructors,
				reflectionConstructors,
				monoCecilConstructor => $"{monoCecilConstructor.Name}({monoCecilConstructor.Parameters.Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				reflectionConstructor => $"{reflectionConstructor.Name}({reflectionConstructor.GetParameters().Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				constructorName => constructorName.StartsWith(".ctor", StringComparison.OrdinalIgnoreCase),
				(reflectionConstructor, monoCecilConstructor) => (ICommonConstructor)new CommonConstructor(monoCecilConstructor, reflectionConstructor));
		}

		internal static ICommonField[] JoinFields(IEnumerable<FieldInfo> reflectionFields, IEnumerable<FieldDefinition> monoCecilFields) {
			return Join(
				monoCecilFields,
				reflectionFields,
				monoCecilField => monoCecilField.Name,
				reflectionField => reflectionField.Name,
				fieldName => true,
				(reflectionField, monoCecilField) => (ICommonField)new CommonField(monoCecilField, reflectionField));
		}

		internal static ICommonMethod[] JoinMethods(IEnumerable<MethodInfo> reflectionMethods, IEnumerable<MethodDefinition> monoCecilMethods) {
			return Join(
				monoCecilMethods,
				reflectionMethods,
				monoCecilMethod => $"{monoCecilMethod.Name}({monoCecilMethod.Parameters.Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				reflectionMethod => $"{reflectionMethod.Name}({reflectionMethod.GetParameters().Select(parameter => parameter.ParameterType.Name).JoinToString(", ")})",
				methodName => !methodName.StartsWith("get_", StringComparison.OrdinalIgnoreCase) && !methodName.StartsWith("set_", StringComparison.OrdinalIgnoreCase),
				(reflectionMethod, monoCecilMethod) => (ICommonMethod)new CommonMethod(monoCecilMethod, reflectionMethod));
		}

		internal static ICommonParameter[] JoinParameters(IEnumerable<ParameterInfo> reflectionParameters, IEnumerable<ParameterDefinition> monoCecilParameters) {
			return Join(
				monoCecilParameters,
				reflectionParameters,
				monoCecilParameter => $"{monoCecilParameter.Name}:{monoCecilParameter.ParameterType.Name}",
				reflectionParameter => $"{reflectionParameter.Name}:{reflectionParameter.ParameterType.Name}",
				parameterName => true,
				(reflectionParameter, monoCecilParameter) => (ICommonParameter)new CommonParameter(monoCecilParameter, reflectionParameter));
		}

		internal static ICommonProperty[] JoinProperties(IEnumerable<PropertyInfo> reflectionProperties, IEnumerable<PropertyDefinition> monoCecilProperties) {
			return Join(
				monoCecilProperties,
				reflectionProperties,
				monoCecilProperty => monoCecilProperty.Name,
				reflectionProperty => reflectionProperty.Name,
				propertyName => true,
				(reflectionProperty, monoCecilProperty) => (ICommonProperty)new CommonProperty(monoCecilProperty, reflectionProperty));
		}

		internal static ICommonType[] JoinTypes(IEnumerable<Type> reflectionTypes, IEnumerable<TypeDefinition> monoCecilTypes) {
			return Join(
				monoCecilTypes,
				reflectionTypes,
				monoCecilType => $"{monoCecilType.FullName}, {monoCecilType.Module.Assembly.FullName}",
				reflectionType => reflectionType.AssemblyQualifiedName,
				typeFullName => true,
				(reflectionType, monoCecilType) => (ICommonType)new CommonType(monoCecilType, reflectionType));
		}

		private static TCommon[] Join<TCommon, TMonoCecil, TReflection>(IEnumerable<TMonoCecil> monoCecils,
																		IEnumerable<TReflection> reflections,
																		Func<TMonoCecil, string> getMonoCecilName,
																		Func<TReflection, string> getReflectionName,
																		Func<string, bool> isSatisfyForSelection,
																		Func<TReflection, TMonoCecil, TCommon> createCommon) {
			var monoCecilAttributesDictionary = monoCecils.ToDictionary(getMonoCecilName);
			var reflectionAttributesDictionary = reflections.ToDictionary(getReflectionName);

			return monoCecilAttributesDictionary.Keys
				.Intersect(reflectionAttributesDictionary.Keys)
				.Where(isSatisfyForSelection)
				.Select(key => createCommon(reflectionAttributesDictionary[key], monoCecilAttributesDictionary[key]))
				.ToArray();
		}
	}
}