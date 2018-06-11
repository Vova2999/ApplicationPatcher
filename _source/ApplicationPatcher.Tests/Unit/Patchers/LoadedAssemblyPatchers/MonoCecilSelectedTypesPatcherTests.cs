using System.Reflection;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers;
using ApplicationPatcher.Tests.FakeTypes;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.LoadedAssemblyPatchers {
	[TestFixture]
	public class MonoCecilSelectedTypesPatcherTests : PatcherTestsBase {
		[Test]
		public void A() {
			var constsType = FakeCommonTypeBuilder.Create("Consts")
				.AddProperty("PublicKey", typeof(string), PropertyMethods.HasGetAndSet)
				.AddMethod("Method", null)
				.Build();

			var typeSystem = CreateMockFor<TypeSystem>();
			typeSystem.Setup(system => system.Void).Returns(() => CreateMockFor<TypeReference>().Object);

			var assembly = FakeCommonAssemblyBuilder.Create().AddCommonType(constsType);
			assembly.MainMonoCecilModuleMock.Setup(module => module.TypeSystem).Returns(() => typeSystem.Object);
			assembly.MainMonoCecilModuleMock.Setup(module => module.ImportReference(It.IsAny<MethodBase>())).Returns(() => CreateMockFor<MethodReference>().Object);

			var applicationPatcherSelfConfiguration = new ApplicationPatcherSelfConfiguration { MonoCecilSelectedPatchingTypeFullNames = new[] { "Consts" } };
			new MonoCecilSelectedTypesPatcher(applicationPatcherSelfConfiguration).Patch(assembly.CommonAssembly);
		}
	}
}