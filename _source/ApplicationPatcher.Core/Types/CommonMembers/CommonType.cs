using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Interfaces;
using Mono.Cecil;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ApplicationPatcher.Core.Types.CommonMembers {
	public class CommonType : CommonMemberBase<CommonType, Type, TypeDefinition>, IHasAttributes, IHasConstructors, IHasFields, IHasMethods, IHasProperties, IHasType {
		public virtual Type Type => GetOrCreate(() => Reflection);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public virtual CommonAttribute[] Attributes { get; private set; }
		public virtual CommonConstructor[] Constructors { get; private set; }
		public virtual CommonField[] Fields { get; private set; }
		public virtual CommonMethod[] Methods { get; private set; }
		public virtual CommonProperty[] Properties { get; private set; }

		internal virtual Dictionary<string, CommonField[]> FieldNameToField { get; private set; }
		Dictionary<string, CommonField[]> IHasFields.FieldNameToField => FieldNameToField;

		internal virtual Dictionary<string, CommonMethod[]> MethodNameToMethod { get; private set; }
		Dictionary<string, CommonMethod[]> IHasMethods.MethodNameToMethod => MethodNameToMethod;

		internal virtual Dictionary<string, CommonProperty[]> PropertyNameToProperty { get; private set; }
		Dictionary<string, CommonProperty[]> IHasProperties.PropertyNameToProperty => PropertyNameToProperty;

		internal virtual Dictionary<Type, CommonAttribute[]> TypeTypeToAttribute { get; private set; }
		Dictionary<Type, CommonAttribute[]> IHasAttributes.TypeTypeToAttribute => TypeTypeToAttribute;

		internal virtual Dictionary<string, CommonAttribute[]> TypeFullNameToAttribute { get; private set; }
		Dictionary<string, CommonAttribute[]> IHasAttributes.TypeFullNameToAttribute => TypeFullNameToAttribute;

		public CommonType(Type reflectionType, TypeDefinition monoCecilType) : base(reflectionType, monoCecilType) {
		}

		IHasAttributes ICommonMember<IHasAttributes>.Load() {
			return Load();
		}
		IHasConstructors ICommonMember<IHasConstructors>.Load() {
			return Load();
		}
		IHasFields ICommonMember<IHasFields>.Load() {
			return Load();
		}
		IHasMethods ICommonMember<IHasMethods>.Load() {
			return Load();
		}
		IHasProperties ICommonMember<IHasProperties>.Load() {
			return Load();
		}
		IHasType ICommonMember<IHasType>.Load() {
			return Load();
		}

		internal override void LoadInternal() {
			base.LoadInternal();

			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Constructors = CommonHelper.JoinConstructors(Reflection.GetConstructors(bindingFlags), MonoCecil.Methods);
			Fields = CommonHelper.JoinFields(Reflection.GetFields(bindingFlags), MonoCecil.Fields);
			Methods = CommonHelper.JoinMethods(Reflection.GetMethods(bindingFlags), MonoCecil.Methods);
			Properties = CommonHelper.JoinProperties(Reflection.GetProperties(bindingFlags), MonoCecil.Properties);

			FieldNameToField = Fields.GroupBy(field => field.Name).ToDictionary(group => group.Key, group => group.ToArray());
			MethodNameToMethod = Methods.GroupBy(method => method.Name).ToDictionary(group => group.Key, group => group.ToArray());
			PropertyNameToProperty = Properties.GroupBy(property => property.Name).ToDictionary(group => group.Key, group => group.ToArray());
			TypeTypeToAttribute = Attributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray());
			TypeFullNameToAttribute = Attributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray());
		}
	}
}