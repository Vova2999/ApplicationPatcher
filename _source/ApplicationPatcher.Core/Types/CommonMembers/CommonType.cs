using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonType : CommonMember<ICommonType, TypeDefinition, Type>, ICommonType {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		public Type Type => GetOrCreate(() => Reflection);

		private ICommonField[] fields;
		public ICommonField[] Fields => fields.CheckLoaded();

		private ICommonMethod[] methods;
		public ICommonMethod[] Methods => methods.CheckLoaded();

		private ICommonProperty[] properties;
		public ICommonProperty[] Properties => properties.CheckLoaded();

		private ICommonAttribute[] attributes;
		public ICommonAttribute[] Attributes => attributes.CheckLoaded();

		private ICommonConstructor[] constructors;
		public ICommonConstructor[] Constructors => constructors.CheckLoaded();

		private IDictionary<string, ICommonField[]> fieldNameToFields;
		public IDictionary<string, ICommonField[]> FieldNameToFields => fieldNameToFields.CheckLoaded();

		private IDictionary<string, ICommonMethod[]> methodNameToMethods;
		public IDictionary<string, ICommonMethod[]> MethodNameToMethods => methodNameToMethods.CheckLoaded();

		private IDictionary<string, ICommonProperty[]> propertyNameToProperties;
		public IDictionary<string, ICommonProperty[]> PropertyNameToProperties => propertyNameToProperties.CheckLoaded();

		private IDictionary<Type, ICommonAttribute[]> typeTypeToAttributes;
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes => typeTypeToAttributes.CheckLoaded();

		private IDictionary<string, ICommonAttribute[]> typeFullNameToAttributes;
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes => typeFullNameToAttributes.CheckLoaded();

		public CommonType(TypeDefinition monoCecilType, Type reflectionType) : base(monoCecilType, reflectionType) {
		}

		protected override void LoadInternal() {
			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			fields = CommonHelper.JoinFields(Reflection.GetFields(bindingFlags), MonoCecil.Fields);
			methods = CommonHelper.JoinMethods(Reflection.GetMethods(bindingFlags), MonoCecil.Methods);
			properties = CommonHelper.JoinProperties(Reflection.GetProperties(bindingFlags), MonoCecil.Properties);
			attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			constructors = CommonHelper.JoinConstructors(Reflection.GetConstructors(bindingFlags), MonoCecil.Methods);

			fieldNameToFields = Fields.GroupBy(field => field.Name).ToDictionary(group => group.Key, group => group.ToArray());
			methodNameToMethods = Methods.GroupBy(method => method.Name).ToDictionary(group => group.Key, group => group.ToArray());
			propertyNameToProperties = Properties.GroupBy(property => property.Name).ToDictionary(group => group.Key, group => group.ToArray());
			typeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			typeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}

		protected override void LoadInDepth(int depth) {
			Fields.ForEach(field => field.Load(depth));
			Methods.ForEach(method => method.Load(depth));
			Properties.ForEach(property => property.Load(depth));
			Attributes.ForEach(attribute => attribute.Load(depth));
			Constructors.ForEach(constructor => constructor.Load(depth));
		}
	}
}