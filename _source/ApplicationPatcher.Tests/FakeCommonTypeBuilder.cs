using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;
using ApplicationPatcher.Tests.FakeTypes;
using Mono.Cecil;
using Mono.Collections.Generic;
using Moq;
using TypeAttributes = System.Reflection.TypeAttributes;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Tests {
	public class FakeCommonTypeBuilder {
		private static IDictionary<object, object> savedMocks = new ConcurrentDictionary<object, object>();
		private static ModuleBuilder moduleBuilder;

		private Type currentType;
		private readonly Type baseType;
		private readonly string typeFullName;
		private readonly List<FakeAttribute> attributes = new List<FakeAttribute>();
		private readonly List<FakeConstructor> constructors = new List<FakeConstructor>();
		private readonly List<FakeField> fields = new List<FakeField>();
		private readonly List<FakeMethod> methods = new List<FakeMethod>();
		private readonly List<FakeProperty> properties = new List<FakeProperty>();

		private FakeCommonTypeBuilder(Type type) {
			currentType = type;
			typeFullName = type.FullName;
		}
		private FakeCommonTypeBuilder(string typeFullName, Type baseType = null) {
			this.typeFullName = typeFullName;
			this.baseType = baseType ?? typeof(object);
		}

		public static FakeCommonTypeBuilder Create(Type type) {
			return new FakeCommonTypeBuilder(type);
		}
		public static FakeCommonTypeBuilder Create(string typeName) {
			return new FakeCommonTypeBuilder(typeName);
		}
		public static FakeCommonTypeBuilder Create(string typeName, Type baseType) {
			return new FakeCommonTypeBuilder(typeName, baseType);
		}
		public static FakeCommonTypeBuilder Create(string typeName, IHasType commonBaseType) {
			return new FakeCommonTypeBuilder(typeName, commonBaseType?.Type);
		}

		public static void ClearCreatedTypes() {
			moduleBuilder = null;
			savedMocks = new ConcurrentDictionary<object, object>();
		}

		public static Mock<TObject> GetMockFor<TObject>(TObject obj) where TObject : class {
			return (Mock<TObject>)savedMocks[obj];
		}
		private static Mock<TObject> CreateMockFor<TObject>(params object[] constructorParameters) where TObject : class {
			var savedMock = new Mock<TObject>(constructorParameters) { CallBase = true };
			return (Mock<TObject>)(savedMocks[savedMock.Object] = savedMock);
		}

		public FakeCommonTypeBuilder AddAttribute(Attribute typeAttribute) {
			return AddAttribute(new FakeAttribute(typeAttribute));
		}
		public FakeCommonTypeBuilder AddAttribute(FakeAttribute typeFakeAttribute) {
			attributes.Add(typeFakeAttribute);
			return this;
		}

		public FakeCommonTypeBuilder AddConstructor(FakeParameter[] parameters) {
			return AddConstructor(parameters, (FakeAttribute[])null);
		}
		public FakeCommonTypeBuilder AddConstructor(FakeParameter[] parameters, params Attribute[] methodAttributes) {
			return AddConstructor(parameters, methodAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddConstructor(FakeParameter[] parameters, params FakeAttribute[] methodFakeAttributes) {
			constructors.Add(new FakeConstructor(parameters, methodFakeAttributes));
			return this;
		}

		public FakeCommonTypeBuilder AddField(string name, Type fieldType) {
			return AddField(name, new FakeType(fieldType), (FakeAttribute[])null);
		}
		public FakeCommonTypeBuilder AddField(string name, Type fieldType, params Attribute[] fieldAttributes) {
			return AddField(name, new FakeType(fieldType), fieldAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddField(string name, Type fieldType, params FakeAttribute[] fieldFakeAttributes) {
			return AddField(name, new FakeType(fieldType), fieldFakeAttributes);
		}
		public FakeCommonTypeBuilder AddField(string name, FakeType fieldType, params Attribute[] fieldAttributes) {
			return AddField(name, fieldType, fieldAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddField(string name, FakeType fieldType, params FakeAttribute[] fieldFakeAttributes) {
			fields.Add(new FakeField(name, fieldType, fieldFakeAttributes));
			return this;
		}

		public FakeCommonTypeBuilder AddMethod(string name, Type returnType, FakeParameter[] parameters) {
			return AddMethod(name, new FakeType(returnType), parameters, (FakeAttribute[])null);
		}
		public FakeCommonTypeBuilder AddMethod(string name, Type returnType, FakeParameter[] parameters, params Attribute[] methodAttributes) {
			return AddMethod(name, new FakeType(returnType), parameters, methodAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddMethod(string name, Type returnType, FakeParameter[] parameters, params FakeAttribute[] methodFakeAttributes) {
			return AddMethod(name, new FakeType(returnType), parameters, methodFakeAttributes);
		}
		public FakeCommonTypeBuilder AddMethod(string name, FakeType returnType, FakeParameter[] parameters) {
			return AddMethod(name, returnType, parameters, (FakeAttribute[])null);
		}
		public FakeCommonTypeBuilder AddMethod(string name, FakeType returnType, FakeParameter[] parameters, params Attribute[] methodAttributes) {
			return AddMethod(name, returnType, parameters, methodAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddMethod(string name, FakeType returnType, FakeParameter[] parameters, params FakeAttribute[] methodFakeAttributes) {
			methods.Add(new FakeMethod(name, returnType, parameters, methodFakeAttributes));
			return this;
		}

		public FakeCommonTypeBuilder AddProperty(string name, Type propertyType, PropertyMethods propertyMethods) {
			return AddProperty(name, new FakeType(propertyType), propertyMethods, (FakeAttribute[])null);
		}
		public FakeCommonTypeBuilder AddProperty(string name, Type propertyType, PropertyMethods propertyMethods, params Attribute[] propertyAttributes) {
			return AddProperty(name, new FakeType(propertyType), propertyMethods, propertyAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddProperty(string name, Type propertyType, PropertyMethods propertyMethods, params FakeAttribute[] propertyFakeAttributes) {
			return AddProperty(name, new FakeType(propertyType), propertyMethods, propertyFakeAttributes);
		}
		public FakeCommonTypeBuilder AddProperty(string name, FakeType propertyType, PropertyMethods propertyMethods, params Attribute[] propertyAttributes) {
			return AddProperty(name, propertyType, propertyMethods, propertyAttributes.Select(attribute => new FakeAttribute(attribute)).ToArray());
		}
		public FakeCommonTypeBuilder AddProperty(string name, FakeType propertyType, PropertyMethods propertyMethods, params FakeAttribute[] propertyFakeAttributes) {
			properties.Add(new FakeProperty(name, propertyType, propertyMethods, propertyFakeAttributes));
			return this;
		}

		public CommonType Build() {
			if (currentType == null)
				currentType = CreateReflectionType(typeFullName, baseType);

			var typeName = typeFullName.Split('.').Last();
			var commonAttributes = CreateCommonAttributes(attributes);
			var commonConstructors = CreateCommonConstructors(constructors, typeFullName);
			var commonFields = CreateCommonFields(fields, typeFullName);
			var commonMethods = CreateCommonMethods(methods, typeFullName);
			var commonProperties = CreateCommonProperties(properties, typeFullName);

			var monoCecilType = CreateMockFor<TypeDefinition>();
			monoCecilType.Setup(type => type.Name).Returns(() => typeName);
			monoCecilType.Setup(type => type.FullName).Returns(() => typeFullName);
			monoCecilType.Setup(type => type.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));
			monoCecilType.Setup(type => type.Fields).Returns(() => new Collection<FieldDefinition>(commonFields.Select(field => field.MonoCecil).ToArray()));
			monoCecilType.Setup(type => type.Methods).Returns(() => new Collection<MethodDefinition>(commonMethods.Select(method => method.MonoCecil).Concat(commonConstructors.Select(constructor => constructor.MonoCecil)).ToArray()));
			monoCecilType.Setup(type => type.Properties).Returns(() => new Collection<PropertyDefinition>(commonProperties.Select(property => property.MonoCecil).ToArray()));

			var commonType = CreateMockFor<CommonType>(currentType, monoCecilType.Object);
			commonType.Setup(type => type.Type).Returns(() => currentType);
			commonType.Setup(type => type.Name).Returns(() => typeName);
			commonType.Setup(type => type.FullName).Returns(() => typeFullName);
			commonType.Setup(type => type.Attributes).Returns(() => commonAttributes);
			commonType.Setup(type => type.Constructors).Returns(() => commonConstructors);
			commonType.Setup(type => type.Fields).Returns(() => commonFields);
			commonType.Setup(type => type.Methods).Returns(() => commonMethods);
			commonType.Setup(type => type.Properties).Returns(() => commonProperties);
			commonType.Setup(type => type.LoadInternal());

			return commonType.Object;
		}
		private static Type CreateReflectionType(FakeType fakeType) {
			return fakeType.Type ?? CreateReflectionType(fakeType.FullName, fakeType.BaseType);
		}
		private static Type CreateReflectionType(string typeFullName, Type baseType) {
			if (moduleBuilder == null) {
				var assemblyName = new AssemblyName("DynamicAssembly");
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
				moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
			}

			var typeBuilder = moduleBuilder.DefineType(typeFullName, TypeAttributes.Public);
			typeBuilder.SetParent(baseType);
			return typeBuilder.CreateType();
		}

		private static CommonAttribute[] CreateCommonAttributes(IEnumerable<FakeAttribute> fakeAttributes) {
			return fakeAttributes?.Select(CreateCommonAttribute).ToArray() ?? new CommonAttribute[0];
		}
		private static CommonAttribute CreateCommonAttribute(FakeAttribute fakeAttribute) {
			var monoCecilAttribute = CreateMockFor<CustomAttribute>();
			var typeReference = CreateTypeReference(new FakeType(fakeAttribute.AttributeType));
			monoCecilAttribute.Setup(attribute => attribute.AttributeType).Returns(() => typeReference);

			var commonAttribute = CreateMockFor<CommonAttribute>(fakeAttribute.AttributeInstance, monoCecilAttribute.Object);
			commonAttribute.Setup(attribute => attribute.Type).Returns(() => fakeAttribute.AttributeType);
			commonAttribute.Setup(attribute => attribute.LoadInternal());

			return commonAttribute.Object;
		}

		private static CommonConstructor[] CreateCommonConstructors(IEnumerable<FakeConstructor> fakeConstructors, string typeFullName) {
			return fakeConstructors?.Select(constructor => CreateCommonConstructor(constructor, typeFullName)).ToArray() ?? new CommonConstructor[0];
		}
		private static CommonConstructor CreateCommonConstructor(FakeConstructor fakeConstructor, string typeFullName) {
			const string constructorName = ".ctor";
			var constructorFullName = $"{typeFullName}::{constructorName}({fakeConstructor.Parameters?.Select(parameter => parameter.ParameterType.FullName).JoinToString(",")})";
			var commonAttributes = CreateCommonAttributes(fakeConstructor.Attributes);
			var commonParameters = CreateCommonParameters(fakeConstructor.Parameters);

			var monoCecilConstructor = CreateMockFor<MethodDefinition>();
			monoCecilConstructor.Setup(constructor => constructor.Name).Returns(() => constructorName);
			monoCecilConstructor.Setup(constructor => constructor.FullName).Returns(() => constructorFullName);
			monoCecilConstructor.Setup(constructor => constructor.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));
			monoCecilConstructor.Setup(constructor => constructor.Parameters).Returns(() => new Collection<ParameterDefinition>(commonParameters.Select(parameter => parameter.MonoCecil).ToArray()));

			var commonConstructor = CreateMockFor<CommonConstructor>(CreateMockFor<ConstructorInfo>().Object, monoCecilConstructor.Object);
			commonConstructor.Setup(constructor => constructor.Attributes).Returns(() => commonAttributes);
			commonConstructor.Setup(constructor => constructor.Parameters).Returns(() => commonParameters);
			commonConstructor.Setup(constructor => constructor.ParameterTypes).Returns(() => commonParameters.Select(parameter => parameter.Type).ToArray());
			commonConstructor.Setup(constructor => constructor.LoadInternal());

			return commonConstructor.Object;
		}

		private static CommonField[] CreateCommonFields(IEnumerable<FakeField> fakeFields, string typeFullName) {
			return fakeFields?.Select(fakeField => CreateCommonField(fakeField, typeFullName)).ToArray() ?? new CommonField[0];
		}
		private static CommonField CreateCommonField(FakeField fakeField, string typeFullName) {
			var fieldName = fakeField.Name;
			var fieldFullName = $"{fakeField.FieldType.FullName} {typeFullName}::{fieldName}()";
			var commonAttributes = CreateCommonAttributes(fakeField.Attributes);

			var monoCecilField = CreateMockFor<FieldDefinition>();
			monoCecilField.Setup(field => field.Name).Returns(() => fieldName);
			monoCecilField.Setup(field => field.FullName).Returns(() => fieldFullName);
			monoCecilField.Setup(field => field.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));

			var commonField = CreateMockFor<CommonField>(CreateMockFor<FieldInfo>().Object, monoCecilField.Object);
			commonField.Setup(field => field.Type).Returns(() => CreateReflectionType(fakeField.FieldType));
			commonField.Setup(field => field.Attributes).Returns(() => commonAttributes);
			commonField.Setup(field => field.LoadInternal());

			return commonField.Object;
		}

		private static CommonMethod[] CreateCommonMethods(IEnumerable<FakeMethod> fakeMethods, string typeFullName) {
			return fakeMethods?.Select(method => CreateCommonMethod(method, typeFullName)).ToArray() ?? new CommonMethod[0];
		}
		private static CommonMethod CreateCommonMethod(FakeMethod fakeMethod, string typeFullName, bool canBeNull = false) {
			if (canBeNull && fakeMethod == null)
				return null;

			var methodName = fakeMethod.Name;
			var methodFullName = $"{typeFullName}::{methodName}({fakeMethod.Parameters?.Select(parameter => parameter.ParameterType.FullName).JoinToString(",")})";
			var commonAttributes = CreateCommonAttributes(fakeMethod.Attributes);
			var commonParameters = CreateCommonParameters(fakeMethod.Parameters);
			var returnTypeReference = CreateTypeReference(fakeMethod.ReturnType);

			var monoCecilMethod = CreateMockFor<MethodDefinition>();
			monoCecilMethod.Setup(method => method.Name).Returns(() => methodName);
			monoCecilMethod.Setup(method => method.FullName).Returns(() => methodFullName);
			monoCecilMethod.Setup(method => method.ReturnType).Returns(() => returnTypeReference);
			monoCecilMethod.Setup(method => method.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));
			monoCecilMethod.Setup(method => method.Parameters).Returns(() => new Collection<ParameterDefinition>(commonParameters.Select(parameter => parameter.MonoCecil).ToArray()));

			var commonMethod = CreateMockFor<CommonMethod>(CreateMockFor<MethodInfo>().Object, monoCecilMethod.Object);
			commonMethod.Setup(method => method.Attributes).Returns(() => commonAttributes);
			commonMethod.Setup(method => method.Parameters).Returns(() => commonParameters);
			commonMethod.Setup(method => method.ReturnType).Returns(() => fakeMethod.ReturnType.Type);
			commonMethod.Setup(method => method.ParameterTypes).Returns(() => commonParameters.Select(parameter => parameter.Type).ToArray());
			commonMethod.Setup(method => method.LoadInternal());

			return commonMethod.Object;
		}

		private static CommonParameter[] CreateCommonParameters(IEnumerable<FakeParameter> fakeParameters) {
			return fakeParameters?.Select(CreateCommonParameter).ToArray() ?? new CommonParameter[0];
		}
		private static CommonParameter CreateCommonParameter(FakeParameter fakeParameter) {
			var reflectionParameter = CreateMockFor<ParameterInfo>();
			reflectionParameter.Setup(parameter => parameter.ParameterType).Returns(() => CreateReflectionType(fakeParameter.ParameterType));

			var monoCecilParameter = CreateMockFor<ParameterDefinition>();
			var typeReference = CreateTypeReference(fakeParameter.ParameterType);
			monoCecilParameter.Setup(parameter => parameter.Name).Returns(() => fakeParameter.Name);
			monoCecilParameter.Setup(parameter => parameter.ParameterType).Returns(() => typeReference);

			var commonParameter = CreateMockFor<CommonParameter>(reflectionParameter.Object, monoCecilParameter.Object);
			commonParameter.Setup(parameter => parameter.Type).Returns(() => CreateReflectionType(fakeParameter.ParameterType));
			commonParameter.Setup(parameter => parameter.LoadInternal());

			return commonParameter.Object;
		}

		private static CommonProperty[] CreateCommonProperties(IEnumerable<FakeProperty> fakeProperties, string typeFullName) {
			return fakeProperties?.Select(property => CreateCommonProperty(property, typeFullName)).ToArray() ?? new CommonProperty[0];
		}
		private static CommonProperty CreateCommonProperty(FakeProperty fakeProperty, string typeFullName) {
			var propertyName = fakeProperty.Name;
			var propertyFullName = $"{typeFullName}.{propertyName}";
			var typeReference = CreateTypeReference(fakeProperty.PropertyType);
			var reflectionType = CreateReflectionType(fakeProperty.PropertyType);
			var commonAttributes = CreateCommonAttributes(fakeProperty.Attributes);
			var propertyGetMethod = CreateCommonMethod(fakeProperty.GetMethod, typeFullName, true);
			var propertySetMethod = CreateCommonMethod(fakeProperty.SetMethod, typeFullName, true);

			var reflectionProperty = CreateMockFor<PropertyInfo>();
			reflectionProperty.Setup(property => property.GetMethod).Returns(() => propertyGetMethod?.Reflection);
			reflectionProperty.Setup(property => property.SetMethod).Returns(() => propertySetMethod?.Reflection);
			reflectionProperty.Setup(property => property.PropertyType).Returns(() => reflectionType);

			var monoCecilProperty = CreateMockFor<PropertyDefinition>();
			monoCecilProperty.Setup(property => property.Name).Returns(() => propertyName);
			monoCecilProperty.Setup(property => property.FullName).Returns(() => propertyFullName);
			monoCecilProperty.Setup(property => property.GetMethod).Returns(() => propertyGetMethod?.MonoCecil);
			monoCecilProperty.Setup(property => property.SetMethod).Returns(() => propertySetMethod?.MonoCecil);
			monoCecilProperty.Setup(property => property.PropertyType).Returns(() => typeReference);
			monoCecilProperty.Setup(property => property.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));

			var commonProperty = CreateMockFor<CommonProperty>(reflectionProperty.Object, monoCecilProperty.Object);
			commonProperty.Setup(property => property.Type).Returns(() => reflectionType);
			commonProperty.Setup(property => property.Attributes).Returns(() => commonAttributes);
			commonProperty.Setup(property => property.GetMethod).Returns(() => propertyGetMethod);
			commonProperty.Setup(property => property.SetMethod).Returns(() => propertySetMethod);
			commonProperty.Setup(property => property.LoadInternal());

			return commonProperty.Object;
		}

		private static TypeReference CreateTypeReference(FakeType fakeType) {
			if (fakeType == null)
				return null;

			var monoCecilTypeReference = CreateMockFor<TypeReference>();
			monoCecilTypeReference.Setup(reference => reference.Name).Returns(() => fakeType.FullName.Split('.').Last());
			monoCecilTypeReference.Setup(reference => reference.FullName).Returns(() => fakeType.FullName);

			return monoCecilTypeReference.Object;
		}
	}
}