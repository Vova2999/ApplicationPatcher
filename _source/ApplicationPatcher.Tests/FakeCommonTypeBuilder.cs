//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using ApplicationPatcher.Core.Types.Common;
//using ApplicationPatcher.Tests.FakeTypes;
//using Mono.Cecil;
//using Mono.Collections.Generic;
//using Moq;
//using TypeAttributes = System.Reflection.TypeAttributes;

//namespace ApplicationPatcher.Tests {
//	public class FakeCommonTypeBuilder {
//		private const string fakeNamespace = "FakeNamespace";

//		private Type currentType;
//		private readonly string typeName;
//		private readonly Type baseType;
//		private ModuleDefinition monoCecilModule;
//		private readonly List<FakeField> fields = new List<FakeField>();
//		private readonly List<FakeMethod> methods = new List<FakeMethod>();
//		private readonly List<FakeProperty> properties = new List<FakeProperty>();
//		private readonly List<FakeAttribute> attributes = new List<FakeAttribute>();

//		private FakeCommonTypeBuilder(Type type) {
//			typeName = type.Name;
//			currentType = type;
//		}

//		private FakeCommonTypeBuilder(string typeName, Type baseType = null) {
//			this.typeName = typeName;
//			this.baseType = baseType ?? typeof(object);
//		}

//		public static FakeCommonTypeBuilder Create(Type type) {
//			return new FakeCommonTypeBuilder(type);
//		}

//		public static FakeCommonTypeBuilder Create(string typeName, Type baseType = null) {
//			return new FakeCommonTypeBuilder(typeName, baseType);
//		}

//		public FakeCommonTypeBuilder AddField(FakeField field) {
//			fields.Add(field);
//			return this;
//		}
//		public FakeCommonTypeBuilder AddMethod(FakeMethod method) {
//			methods.Add(method);
//			return this;
//		}
//		public FakeCommonTypeBuilder AddProperty(FakeProperty property) {
//			properties.Add(property);
//			return this;
//		}
//		public FakeCommonTypeBuilder AddAttribute(FakeAttribute attribute) {
//			attributes.Add(attribute);
//			return this;
//		}

//		public FakeCommonTypeBuilder WhereFrom(ModuleDefinition monoCecilModule) {
//			if (this.monoCecilModule != null)
//				throw new ArgumentException("Module already saved");

//			this.monoCecilModule = monoCecilModule;
//			return this;
//		}

//		public CommonType Build() {
//			if (currentType == null) {
//				var assemblyName = new AssemblyName("DynamicAssembly");
//				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
//				var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
//				var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
//				typeBuilder.SetParent(baseType);
//				currentType = typeBuilder.CreateType();
//			}

//			var monoCecilType = new Mock<TypeDefinition>(null);
//			monoCecilType.Setup(type => type.Name).Returns(() => typeName);
//			monoCecilType.Setup(type => type.FullName).Returns(() => CreateFullName(typeName));
//			if (monoCecilModule != null)
//				monoCecilType.Setup(type => type.Module).Returns(() => monoCecilModule);

//			var reflectionType = new Mock<Type>(currentType);
//			reflectionType.Setup(type => type.Name).Returns(() => typeName);
//			reflectionType.Setup(type => type.FullName).Returns(() => CreateFullName(typeName));

//			var commonType = new Mock<CommonType>(monoCecilType.Object, reflectionType.Object);
//			commonType.Setup(type => type.Name).Returns(() => typeName);
//			commonType.Setup(type => type.FullName).Returns(() => CreateFullName(typeName));

//			CreateFields(commonType, monoCecilType, reflectionType);
//			CreateMethods(commonType, monoCecilType, reflectionType);
//			CreateProperties(commonType, monoCecilType, reflectionType);
//			CreateAttributes(commonType, monoCecilType, reflectionType);

//			return commonType.Object;
//		}

//		private void CreateFields(Mock<CommonType> commonType, Mock<TypeDefinition> monoCecilType, Mock<Type> reflectionType) {
//			var commonFields = (fields?.Select(field => CreateCommonField(commonType, field)) ?? Enumerable.Empty<CommonField>()).ToArray();

//			commonType.Setup(type => type.Fields).Returns(() => commonFields);
//			monoCecilType.Setup(type => type.Fields).Returns(() => new Collection<FieldDefinition>(commonFields.Select(field => field.MonoCecilField).ToArray()));
//			reflectionType.Setup(type => type.GetFields()).Returns(() => commonFields.Select(field => field.ReflectionField).ToArray());
//		}
//		private static CommonField CreateCommonField(Mock<CommonType> commonType, FakeField fakeField) {
//			if (fakeField == null)
//				return null;

//			var fieldName = fakeField.Name;
//			var fieldFullName = $"{commonType.Object.FullName}.{fieldName}";
//			var commonAttributes = CreateCommonAttributes(fakeField.Attributes);

//			var monoCecilField = new Mock<FieldDefinition>(null);
//			monoCecilField.Setup(field => field.Name).Returns(() => fieldName);
//			monoCecilField.Setup(field => field.FullName).Returns(() => fieldFullName);
//			monoCecilField.Setup(field => field.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecilAttribute).ToArray()));

//			var reflectionField = new Mock<FieldInfo>(null);
//			reflectionField.Setup(field => field.Name).Returns(() => fieldName);
//			reflectionField.Setup(field => field.GetCustomAttributes()).Returns(() => commonAttributes.Select(attribute => attribute.ReflectionAttribute).ToArray());

//			return new Mock<CommonField>(commonAttributes, monoCecilField.Object, reflectionField.Object).Object;
//		}

//		private void CreateMethods(Mock<CommonType> commonType, Mock<TypeDefinition> monoCecilType, Mock<Type> reflectionType) {
//			var commonMethods = (methods?.Select(method => CreateCommonMethod(commonType, method)) ?? Enumerable.Empty<CommonMethod>()).ToArray();

//			commonType.Setup(type => type.Methods).Returns(() => commonMethods);
//			monoCecilType.Setup(type => type.Methods).Returns(() => new Collection<MethodDefinition>(commonMethods.Select(field => field.MonoCecilMethod).ToArray()));
//			reflectionType.Setup(type => type.GetMethods()).Returns(() => commonMethods.Select(field => field.ReflectionMethod).ToArray());
//		}
//		private static CommonMethod CreateCommonMethod(Mock<CommonType> commonType, FakeMethod fakeMethod) {
//			if (fakeMethod == null)
//				return null;

//			var methodName = fakeMethod.Name;
//			var methodFullName = $"{commonType.Object.FullName}.{methodName}";
//			var commonParameters = CreateCommonParameters(fakeMethod.Parameters);
//			var commonAttributes = CreateCommonAttributes(fakeMethod.Attributes);

//			var monoCecilMethod = new Mock<MethodDefinition>(null);
//			monoCecilMethod.Setup(method => method.Name).Returns(() => methodName);
//			monoCecilMethod.Setup(method => method.FullName).Returns(() => methodFullName);
//			monoCecilMethod.Setup(method => method.Parameters).Returns(() => new Collection<ParameterDefinition>(commonParameters.Select(field => field.MonoCecilParameter).ToArray()));
//			monoCecilMethod.Setup(method => method.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(field => field.MonoCecilAttribute).ToArray()));

//			var reflectionMethod = new Mock<MethodInfo>(null);
//			reflectionMethod.Setup(method => method.Name).Returns(() => methodName);
//			reflectionMethod.Setup(method => method.GetParameters()).Returns(() => commonParameters.Select(field => field.ReflectionParameter).ToArray());
//			reflectionMethod.Setup(method => method.GetCustomAttributes()).Returns(() => commonAttributes.Select(field => field.ReflectionAttribute).ToArray());

//			return new Mock<CommonMethod>(commonAttributes, monoCecilMethod.Object, reflectionMethod.Object).Object;
//		}
//		private static CommonParameter[] CreateCommonParameters(IEnumerable<FakeParameter> fakeParameters) {
//			return fakeParameters?.Select(CreateCommonParameter).ToArray();
//		}
//		private static CommonParameter CreateCommonParameter(FakeParameter fakeParameter) {
//			if (fakeParameter == null)
//				return null;

//			var monoCecilParameter = new Mock<ParameterDefinition>(null);
//			monoCecilParameter.Setup(parameter => parameter.Name).Returns(() => fakeParameter.Name);
//			monoCecilParameter.Setup(parameter => parameter.ParameterType).Returns(() => CreateTypeDefinitionReference(fakeParameter.ParameterType));

//			var reflectionParameter = new Mock<ParameterInfo>(null);
//			reflectionParameter.Setup(parameter => parameter.Name).Returns(() => fakeParameter.Name);
//			reflectionParameter.Setup(parameter => parameter.ParameterType).Returns(() => CreateType(fakeParameter.ParameterType));

//			return new Mock<CommonParameter>(monoCecilParameter.Object, reflectionParameter.Object).Object;
//		}

//		private void CreateProperties(Mock<CommonType> commonType, Mock<TypeDefinition> monoCecilType, Mock<Type> reflectionType) {
//			var commonProperties = (properties?.Select(property => CreateCommonProperty(commonType, property)) ?? Enumerable.Empty<CommonProperty>()).ToArray();

//			commonType.Setup(type => type.Properties).Returns(() => commonProperties);
//			monoCecilType.Setup(type => type.Properties).Returns(() => new Collection<PropertyDefinition>(commonProperties.Select(field => field.MonoCecilProperty).ToArray()));
//			reflectionType.Setup(type => type.GetProperties()).Returns(() => commonProperties.Select(field => field.ReflectionProperty).ToArray());
//		}
//		private static CommonProperty CreateCommonProperty(Mock<CommonType> commonType, FakeProperty fakeProperty) {
//			if (fakeProperty == null)
//				return null;

//			var propertyName = fakeProperty.Name;
//			var propertyFullName = CreateFullName(propertyName);

//			if (fakeProperty.GetMethod != null)
//				fakeProperty.GetMethod.Name = $"get_{propertyName}";
//			if (fakeProperty.SetMethod != null)
//				fakeProperty.SetMethod.Name = $"set_{propertyName}";

//			var commonAttributes = CreateCommonAttributes(fakeProperty.Attributes);
//			var propertyGetMethod = CreateCommonMethod(commonType, fakeProperty.GetMethod);
//			var propertySetMethod = CreateCommonMethod(commonType, fakeProperty.SetMethod);

//			var monoCecilProperty = new Mock<PropertyDefinition>(null);
//			monoCecilProperty.Setup(property => property.Name).Returns(() => propertyName);
//			monoCecilProperty.Setup(property => property.FullName).Returns(() => propertyFullName);
//			monoCecilProperty.Setup(property => property.GetMethod).Returns(() => propertyGetMethod?.MonoCecilMethod);
//			monoCecilProperty.Setup(property => property.SetMethod).Returns(() => propertySetMethod?.MonoCecilMethod);
//			monoCecilProperty.Setup(property => property.PropertyType).Returns(() => CreateTypeDefinitionReference(fakeProperty.PropertyType));
//			monoCecilProperty.Setup(property => property.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecilAttribute).ToArray()));

//			var reflectionProperty = new Mock<PropertyInfo>(null);
//			reflectionProperty.Setup(property => property.Name).Returns(() => propertyName);
//			reflectionProperty.Setup(property => property.GetMethod).Returns(() => propertyGetMethod?.ReflectionMethod);
//			reflectionProperty.Setup(property => property.SetMethod).Returns(() => propertySetMethod?.ReflectionMethod);
//			reflectionProperty.Setup(property => property.PropertyType).Returns(() => CreateType(fakeProperty.PropertyType));
//			reflectionProperty.Setup(property => property.GetCustomAttributes()).Returns(() => commonAttributes.Select(attribute => attribute.ReflectionAttribute));

//			return new Mock<CommonProperty>(commonAttributes, monoCecilProperty.Object, reflectionProperty.Object).Object;
//		}

//		private void CreateAttributes(Mock<CommonType> commonType, Mock<TypeDefinition> monoCecilType, Mock<Type> reflectionType) {
//			var commonAttributes = CreateCommonAttributes(attributes);

//			commonType.Setup(type => type.Attributes).Returns(() => commonAttributes);
//			monoCecilType.Setup(type => type.CustomAttributes).Returns(() => new Collection<CustomAttribute>(commonAttributes.Select(field => field.MonoCecilAttribute).ToArray().ToArray()));
//			reflectionType.Setup(type => type.GetCustomAttributes()).Returns(() => commonAttributes.Select(field => field.ReflectionAttribute));
//		}
//		private static CommonAttribute[] CreateCommonAttributes(IEnumerable<FakeAttribute> fakeAttributes) {
//			return (fakeAttributes?.Select(CreateCommonAttribute) ?? Enumerable.Empty<CommonAttribute>()).ToArray();
//		}
//		private static CommonAttribute CreateCommonAttribute(FakeAttribute fakeAttribute) {
//			if (fakeAttribute == null)
//				return null;

//			var monoCecilAttribute = new Mock<CustomAttribute>(null);
//			monoCecilAttribute.Setup(attribute => attribute.AttributeType).Returns(() => CreateTypeDefinitionReference(new FakeType(fakeAttribute.AttributeType)));

//			var reflectionAttribute = fakeAttribute.AttributeInstance;

//			return new Mock<CommonAttribute>(monoCecilAttribute.Object, reflectionAttribute).Object;
//		}

//		private static string CreateFullName(string name) {
//			return $"{fakeNamespace}.{name}";
//		}
//		private static TypeReference CreateTypeDefinitionReference(FakeType fakeType) {
//			if (fakeType == null)
//				return null;

//			var monoCecilTypeReference = new Mock<TypeReference>(null);
//			monoCecilTypeReference.Setup(reference => reference.Name).Returns(() => fakeType.Name);
//			monoCecilTypeReference.Setup(reference => reference.FullName).Returns(() => fakeType.FullName);

//			return monoCecilTypeReference.Object;
//		}
//		private static Type CreateType(FakeType fakeType) {
//			if (fakeType == null)
//				return null;

//			var reflectionType = new Mock<Type>(fakeType.Type);
//			reflectionType.Setup(reference => reference.Name).Returns(() => fakeType.Name);
//			reflectionType.Setup(reference => reference.FullName).Returns(() => fakeType.FullName);

//			return reflectionType.Object;
//		}
//	}
//}