using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.OnLoadedAssembly;
using ApplicationPatcher.Tests.FakeTypes;
using FluentAssertions;
using Mono.Cecil;
using Mono.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.OnLoadedAssembly {
	[TestFixture]
	public class MonoCecilSelectedTypesPatcherTests : PatcherTestsBase {
		private FakeCommonAssemblyBuilder assembly;
		private ICommonType[] otherTypes;

		[SetUp]
		public void SetUp() {
			const int otherTypesCount = 3;

			otherTypes = Enumerable.Range(0, otherTypesCount).Select(x => FakeCommonTypeBuilder.Create($"OtherType{x}").Build()).ToArray();

			var typeSystem = CreateMockFor<TypeSystem>();
			typeSystem.Setup(system => system.Void).Returns(() => CreateMockFor<TypeReference>().Object);

			assembly = FakeCommonAssemblyBuilder.Create().AddCommonTypes(otherTypes);
			assembly.MainMonoCecilModuleMock.Setup(module => module.TypeSystem).Returns(() => typeSystem.Object);
			assembly.MainMonoCecilModuleMock.Setup(module => module.ImportReference(It.IsAny<MethodBase>())).Returns(() => CreateMockFor<MethodReference>().Object);
		}

		[TearDown]
		public void CheckOtherTypes() {
			otherTypes.Select(FakeCommonTypeBuilder.GetMockFor).ForEach(otherTypeMock => otherTypeMock.Verify(mock => mock.Load(), Times.Never));
		}

		[Test]
		public void HaveSelectedTypes_WithEmptyConstructor() {
			var typeWithoutEmptyConstructor = FakeCommonTypeBuilder.Create("TypeWithConstructor").AddConstructor(null).Build();

			Patch(typeWithoutEmptyConstructor);
			CheckSetInternalMethod(typeWithoutEmptyConstructor.GetConstructor().MonoCecil);
		}

		[Test]
		public void HaveSelectedTypes_WithoutEmptyConstructor() {
			var typeWithoutEmptyConstructor = FakeCommonTypeBuilder.Create("TypeWithoutConstructor").Build();
			FakeCommonTypeBuilder.GetMockFor(typeWithoutEmptyConstructor.MonoCecil).Setup(type => type.Methods).Returns(new Collection<MethodDefinition>());

			Patch(typeWithoutEmptyConstructor);
			typeWithoutEmptyConstructor.MonoCecil.Methods.Should().HaveCount(1).And.Subject.First().Name.Should().Be(".ctor");
		}

		[Test]
		public void HaveSelectedTypes_WithProperties() {
			var typeWithProperties = FakeCommonTypeBuilder.Create("TypeWithProperties")
				.AddProperty("FirstProperty", typeof(int), PropertyMethods.HasGetAndSet)
				.AddProperty("SecondProperty", typeof(bool), PropertyMethods.HasGetOnly)
				.Build();

			Patch(typeWithProperties);
			CheckSetVirtualMethod(typeWithProperties.GetProperty("FirstProperty").MonoCecil.GetMethod);
			CheckSetVirtualMethod(typeWithProperties.GetProperty("FirstProperty").MonoCecil.SetMethod);
			CheckSetVirtualMethod(typeWithProperties.GetProperty("SecondProperty").MonoCecil.GetMethod);
		}

		[Test]
		public void HaveSelectedTypes_WithMethods() {
			var typeWithMethods = FakeCommonTypeBuilder.Create("TypeWithMethods")
				.AddMethod("FirstMethod", typeof(void), null)
				.AddMethod("SecondMethod", typeof(void), null)
				.Build();

			Patch(typeWithMethods);
			CheckSetVirtualMethod(typeWithMethods.GetMethod("FirstMethod").MonoCecil);
			CheckSetVirtualMethod(typeWithMethods.GetMethod("SecondMethod").MonoCecil);
		}

		[Test]
		public void HaveSelectedTypes_WithoutEmptyConstructor_WithProperties_WithMethods() {
			var myType = FakeCommonTypeBuilder.Create("MyType")
				.AddProperty("FirstProperty", typeof(int), PropertyMethods.HasGetAndSet)
				.AddProperty("SecondProperty", typeof(bool), PropertyMethods.HasGetOnly)
				.AddMethod("FirstMethod", typeof(void), null)
				.AddMethod("SecondMethod", typeof(void), null)
				.Build();

			FakeCommonTypeBuilder.GetMockFor(myType.MonoCecil).Setup(type => type.Methods).Returns(new Collection<MethodDefinition>());

			Patch(myType);
			CheckSetVirtualMethod(myType.GetProperty("FirstProperty").MonoCecil.GetMethod);
			CheckSetVirtualMethod(myType.GetProperty("FirstProperty").MonoCecil.SetMethod);
			CheckSetVirtualMethod(myType.GetProperty("SecondProperty").MonoCecil.GetMethod);
			CheckSetVirtualMethod(myType.GetMethod("FirstMethod").MonoCecil);
			CheckSetVirtualMethod(myType.GetMethod("SecondMethod").MonoCecil);
			myType.MonoCecil.Methods.Should().HaveCount(1).And.Subject.First().Name.Should().Be(".ctor");
		}

		[Test]
		public void NotHaveSelectedTypes() {
			Patch(null);
		}

		private void Patch(ICommonType type) {
			if (type != null)
				assembly.AddCommonType(type);

			new MonoCecilSelectedTypesPatcher(new ApplicationPatcherSelfConfiguration {
				MonoCecilSelectedPatchingTypeFullNames = new[] { type?.Name, "AnyName" }.Where(typeName => !typeName.IsNullOrEmpty()).ToArray()
			}).Patch(assembly.CommonAssembly);
		}

		private static void CheckSetInternalMethod(MethodDefinition monoCecilMethod) {
			FakeCommonTypeBuilder.GetMockFor(monoCecilMethod).VerifySet(method => method.IsAssembly = true, Times.Once);
			FakeCommonTypeBuilder.GetMockFor(monoCecilMethod).VerifySet(method => method.IsAssembly = It.IsAny<bool>(), Times.Once);
		}

		private static void CheckSetVirtualMethod(MethodDefinition monoCecilMethod) {
			FakeCommonTypeBuilder.GetMockFor(monoCecilMethod).VerifySet(method => method.IsVirtual = true, Times.Once);
			FakeCommonTypeBuilder.GetMockFor(monoCecilMethod).VerifySet(method => method.IsVirtual = It.IsAny<bool>(), Times.Once);
		}
	}
}