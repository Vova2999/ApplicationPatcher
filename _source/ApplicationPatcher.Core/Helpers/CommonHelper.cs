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
				reflectionType => reflectionType.FullName,
				monoCecilType => monoCecilType.FullName,
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
				reflectionMethod => reflectionMethod.Name,
				monoCecilMethod => monoCecilMethod.Name,
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
			var reflectionAttributesDictionary = CreateDictionary(reflections, getReflectionName);
			var monoCecilAttributesDictionary = CreateDictionary(monoCecils, getMonoCecilName);

			return reflectionAttributesDictionary.Keys
				.Intersect(monoCecilAttributesDictionary.Keys)
				.Where(isSatisfyForSelection)
				.Select(key => createCommon(reflectionAttributesDictionary[key], monoCecilAttributesDictionary[key]))
				.ToArray();
		}
		private static Dictionary<string, TItem> CreateDictionary<TItem>(IEnumerable<TItem> items, Func<TItem, string> getItemName) {
			return items
				.GroupBy(getItemName)
				.Where(x => x.Count() == 1)
				.ToDictionary(x => x.Key, x => x.Single());
		}
	}
}