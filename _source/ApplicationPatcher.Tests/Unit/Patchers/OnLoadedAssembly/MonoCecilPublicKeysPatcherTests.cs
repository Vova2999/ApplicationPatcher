using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.OnLoadedAssembly;
using FluentAssertions;
using Mono.Cecil;
using Mono.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.OnLoadedAssembly {
	[TestFixture]
	public class MonoCecilPublicKeysPatcherTests : PatcherTestsBase {
		private FakeCommonAssemblyBuilder assembly;
		private Mock<AssemblyNameDefinition> assemblyName;
		private ICommonAttribute[] applicationCommonAttributes;
		private ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;

		[SetUp]
		public void SetUp() {
			applicationCommonAttributes = new ICommonAttribute[0];
			assemblyName = CreateMockFor<AssemblyNameDefinition>();

			assembly = FakeCommonAssemblyBuilder.Create();
			assembly.MainMonoCecilAssemblyMock.Setup(a => a.Name).Returns(assemblyName.Object);
			assembly.CommonAssemblyMock.Setup(commonAssembly => commonAssembly.Attributes).Returns(() => applicationCommonAttributes);

			applicationPatcherSelfConfiguration = new ApplicationPatcherSelfConfiguration {
				MonoCecilSelectedAssemblyReferenceNames = new string[0],
				MonoCecilSelectedInternalsVisibleToAttributeNames = new string[0],
				MonoCecilNewPublicKey = new byte[0]
			};
		}

		[Test]
		public void RewriteMainAssemblyPublicKey() {
			applicationPatcherSelfConfiguration.MonoCecilNewPublicKey = new byte[] { 1, 2, 3 };
			applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken = new byte[] { 3, 2, 1 };

			Patch();
			assemblyName.VerifySet(name => name.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey, Times.Once);
			assemblyName.VerifySet(name => name.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken, Times.Once);
		}

		[Test]
		public void RewriteSelectedAssemblyReferencesPublicKey() {
			const int usedAssemblyReferencesCount = 3, notUsedAssemblyReferencesCount = 2;
			var usedAssemblyReferences = CreateAssemblyReferences("Used", usedAssemblyReferencesCount);
			var notUsedAssemblyReferences = CreateAssemblyReferences("NotUsed", notUsedAssemblyReferencesCount);

			assembly.MainMonoCecilModuleMock.Setup(module => module.AssemblyReferences)
				.Returns(() => new Collection<AssemblyNameReference>(usedAssemblyReferences.Concat(notUsedAssemblyReferences).Select(reference => reference.Object).ToArray()));

			applicationPatcherSelfConfiguration.MonoCecilSelectedAssemblyReferenceNames = usedAssemblyReferences.Select(reference => reference.Object.Name).Concat(new[] { "AnyAssemblyName" }).ToArray();
			applicationPatcherSelfConfiguration.MonoCecilNewPublicKey = new byte[] { 1, 2, 3 };
			applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken = new byte[] { 3, 2, 1 };

			Patch();

			foreach (var usedAssemblyReference in usedAssemblyReferences) {
				usedAssemblyReference.VerifySet(reference => reference.PublicKey = It.IsAny<byte[]>(), Times.Once);
				usedAssemblyReference.VerifySet(reference => reference.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey, Times.Once);
				usedAssemblyReference.VerifySet(reference => reference.PublicKeyToken = It.IsAny<byte[]>(), Times.Once);
				usedAssemblyReference.VerifySet(reference => reference.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken, Times.Once);
			}

			foreach (var notUsedAssemblyReference in notUsedAssemblyReferences) {
				notUsedAssemblyReference.VerifySet(reference => reference.PublicKey = It.IsAny<byte[]>(), Times.Never);
				notUsedAssemblyReference.VerifySet(reference => reference.PublicKeyToken = It.IsAny<byte[]>(), Times.Never);
			}
		}

		[Test]
		public void RewriteOrCreateInternalVisibleToAttributes() {
			var usedInternalsVisibleToAttributes = new[] {
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil1"),
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil2, PublicKey=000000"),
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil3"),
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil4, PublicKey=123123")
			};

			var notUsedInternalsVisibleToAttributes = new[] {
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil5"),
				CreateCommonInternalsVisibleToAttribute("Mono.Cecil6, PublicKey=343434")
			};

			var commonAttributes = usedInternalsVisibleToAttributes.Concat(notUsedInternalsVisibleToAttributes).ToArray();
			assembly.AddCommonAttributes(commonAttributes);
			assembly.CommonAssemblyMock.Setup(commonAssembly => commonAssembly.Attributes).Returns(commonAttributes);
			assembly.MainMonoCecilAssemblyMock.Setup(a => a.CustomAttributes).Returns(new Collection<CustomAttribute>(commonAttributes.Select(attribute => attribute.MonoCecil).ToArray()));

			applicationPatcherSelfConfiguration.MonoCecilNewPublicKey = new byte[] { 1, 2, 3 };
			applicationPatcherSelfConfiguration.MonoCecilSelectedInternalsVisibleToAttributeNames = new[] { "Mono.Cecil1", "Mono.Cecil2", "Mono.Cecil3", "Mono.Cecil4" };

			Patch();

			assembly.MainMonoCecilAssembly.CustomAttributes.Should()
				.HaveCount(7)
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && (attribute.ConstructorArguments.First().Value as string).StartsWith("DynamicProxyGenAssembly2", StringComparison.Ordinal))
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil1, PublicKey=010203")
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil2, PublicKey=010203")
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil3, PublicKey=010203")
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil4, PublicKey=010203")
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil5")
				.And.Contain(attribute => attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments.First().Value as string == "Mono.Cecil6, PublicKey=343434");
		}

		private void Patch() {
			new MonoCecilPublicKeysPatcher(applicationPatcherSelfConfiguration).Patch(assembly.CommonAssembly);
		}

		private static Mock<AssemblyNameReference>[] CreateAssemblyReferences(string assemblyNamePrefix, int assemblyReferencesCount) {
			return Enumerable.Range(0, assemblyReferencesCount)
				.Select(x => {
					var assemblyNameReference = CreateMockFor<AssemblyNameReference>();
					assemblyNameReference.Setup(reference => reference.Name).Returns($"{assemblyNamePrefix}AssemblyName{x}");
					return assemblyNameReference;
				})
				.ToArray();
		}

		private static ICommonAttribute CreateCommonInternalsVisibleToAttribute(string internalsVisibleToAssemblyName) {
			var internalsVisibleToAttribute = new InternalsVisibleToAttribute(internalsVisibleToAssemblyName);

			var customAttribute = CreateMockFor<CustomAttribute>();
			var customAttributeArgument = new CustomAttributeArgument(CreateMockFor<TypeReference>().Object, internalsVisibleToAssemblyName);
			customAttribute.Setup(attribute => attribute.ConstructorArguments).Returns(new Collection<CustomAttributeArgument>(new[] { customAttributeArgument }));

			var commonAttribute = CreateMockFor<ICommonAttribute>();
			commonAttribute.Setup(attribute => attribute.MonoCecil).Returns(() => customAttribute.Object);
			commonAttribute.Setup(attribute => attribute.Reflection).Returns(() => internalsVisibleToAttribute);
			commonAttribute.Setup(attribute => attribute.Type).Returns(() => typeof(InternalsVisibleToAttribute));
			commonAttribute.Setup(attribute => attribute.FullName).Returns(() => typeof(InternalsVisibleToAttribute).FullName);

			return commonAttribute.Object;
		}
	}
}