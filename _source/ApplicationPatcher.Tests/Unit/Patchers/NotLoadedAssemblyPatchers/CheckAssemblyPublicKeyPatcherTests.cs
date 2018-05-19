using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Types.Common;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.NotLoadedAssemblyPatchers;
using FluentAssertions;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.NotLoadedAssemblyPatchers {
	[TestFixture]
	public class CheckAssemblyPublicKeyPatcherTests {
		[Test]
		public void A() {
			var assemblyNameDefinition = new Mock<AssemblyNameDefinition>(MockBehavior.Strict);
			assemblyNameDefinition.Setup(assemblyName => assemblyName.PublicKeyToken).Returns(new byte[] { 1, 2, 3 });

			var mainMonoCecilAssembly = new Mock<AssemblyDefinition>(MockBehavior.Strict);
			mainMonoCecilAssembly.Setup(assembly => assembly.Name).Returns(assemblyNameDefinition.Object);

			var commonAssembly = new Mock<CommonAssembly>(MockBehavior.Strict, null, null, null, null, false);
			commonAssembly.Setup(assembly => assembly.MainMonoCecilAssembly).Returns(mainMonoCecilAssembly.Object);

			var checkAssemblyPublicKeyPatcher = new CheckAssemblyPublicKeyPatcher(new ApplicationPatcherSelfConfiguration());
			var patchResult = checkAssemblyPublicKeyPatcher.Patch(commonAssembly.Object);

			patchResult.Should().Be(PatchResult.Succeeded);
		}
	}
}