using ApplicationPatcher.Core;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.NotLoadedAssemblyPatchers;
using FluentAssertions;
using Mono.Cecil;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.NotLoadedAssemblyPatchers {
	[TestFixture]
	public class CheckAssemblyPublicKeyPatcherTests : PatcherTestsBase {
		[Test]
		public void EqualPublicKeyTokens() {
			CheckPublicKeyTokens(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, PatchResult.Cancel);
		}

		[Test]
		public void NotEqualPublicKeyTokens() {
			CheckPublicKeyTokens(new byte[] { 1, 2, 3 }, new byte[] { 1, 3, 2 }, PatchResult.Continue);
		}

		private static void CheckPublicKeyTokens(byte[] assemblyPublicKeyToken, byte[] configurationPublicKeyToken, PatchResult expectedPatchResult) {
			var assemblyNameDefinition = CreateMockFor<AssemblyNameDefinition>();
			assemblyNameDefinition.Setup(assemblyName => assemblyName.PublicKeyToken).Returns(assemblyPublicKeyToken);

			var assembly = FakeCommonAssemblyBuilder.Create();
			assembly.MainMonoCecilAssemblyMock.Setup(mainMonoCecilAssembly => mainMonoCecilAssembly.Name).Returns(assemblyNameDefinition.Object);

			var applicationPatcherSelfConfiguration = new ApplicationPatcherSelfConfiguration { MonoCecilNewPublicKeyToken = configurationPublicKeyToken };
			var checkAssemblyPublicKeyPatcher = new CheckAssemblyPublicKeyPatcher(applicationPatcherSelfConfiguration);
			var patchResult = checkAssemblyPublicKeyPatcher.Patch(assembly.CommonAssembly);

			patchResult.Should().Be(expectedPatchResult);
		}
	}
}