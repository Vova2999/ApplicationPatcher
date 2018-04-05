using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.Common;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Helpers {
	internal static class CommonHelper {
		internal static CommonType[] JoinTypes(IEnumerable<Type> reflectionTypes, IEnumerable<TypeDefinition> monoCecilTypes) {
			return Join(
				reflectionTypes,
				monoCecilTypes,
				reflectionType => reflectionType.FullName ?? Guid.NewGuid().ToString(),
				monoCecilType => monoCecilType.FullName ?? Guid.NewGuid().ToString(),
				typeFullName => true,
				(reflectionType, monoCecilType) => new CommonType(reflectionType, monoCecilType));
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
				reflectionMethod => $"{reflectionMethod.Name}({string.Join(", ", reflectionMethod.GetParameters().Select(parameter => parameter.ParameterType.Name))})",
				monoCecilMethod => $"{monoCecilMethod.Name}({string.Join(", ", monoCecilMethod.Parameters.Select(parameter => parameter.ParameterType.Name))})",
				methodName => methodName != ".ctor" && !methodName.StartsWith("get_") && !methodName.StartsWith("set_"),
				(reflectionMethod, monoCecilMethod) => new CommonMethod(reflectionMethod, monoCecilMethod));
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

		internal static CommonAttribute[] JoinAttributes(IEnumerable<Attribute> reflectionAttributes, IEnumerable<CustomAttribute> monoCecilAttributes) {
			return Join(
				reflectionAttributes,
				monoCecilAttributes,
				reflectionAttribute => reflectionAttribute.GetType().FullName,
				monoCecilAttribute => monoCecilAttribute.AttributeType.FullName,
				attributeFullName => true,
				(reflectionAttribute, monoCecilAttribute) => new CommonAttribute(reflectionAttribute, monoCecilAttribute));
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