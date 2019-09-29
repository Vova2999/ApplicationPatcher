using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonType : CommonMember<ICommonType, TypeDefinition, Type>, ICommonType {
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);

		public Type Type => GetOrCreate(() => Reflection);
		public ICommonField[] Fields { get; private set; }
		public ICommonMethod[] Methods { get; private set; }
		public ICommonProperty[] Properties { get; private set; }
		public ICommonAttribute[] Attributes { get; private set; }
		public ICommonConstructor[] Constructors { get; private set; }
		public IDictionary<string, ICommonField[]> FieldNameToFields { get; private set; }
		public IDictionary<string, ICommonMethod[]> MethodNameToMethods { get; private set; }
		public IDictionary<string, ICommonProperty[]> PropertyNameToProperties { get; private set; }
		public IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes { get; private set; }
		public IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes { get; private set; }

		public CommonType(TypeDefinition monoCecilType, Type reflectionType) : base(monoCecilType, reflectionType) {
		}

		protected override void LoadInternal() {
			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Constructors = CommonHelper.JoinConstructors(Reflection.GetConstructors(bindingFlags), MonoCecil.Methods);
			Fields = CommonHelper.JoinFields(Reflection.GetFields(bindingFlags), MonoCecil.Fields);
			Methods = CommonHelper.JoinMethods(Reflection.GetMethods(bindingFlags), MonoCecil.Methods);
			Properties = CommonHelper.JoinProperties(Reflection.GetProperties(bindingFlags), MonoCecil.Properties);

			FieldNameToFields = Fields.GroupBy(field => field.Name).ToDictionary(group => group.Key, group => group.ToArray());
			MethodNameToMethods = Methods.GroupBy(method => method.Name).ToDictionary(group => group.Key, group => group.ToArray());
			PropertyNameToProperties = Properties.GroupBy(property => property.Name).ToDictionary(group => group.Key, group => group.ToArray());
			TypeTypeToAttributes = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttributes = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}