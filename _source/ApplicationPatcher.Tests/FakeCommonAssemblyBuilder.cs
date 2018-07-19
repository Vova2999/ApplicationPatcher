using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;
using Mono.Cecil;
using Moq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Tests {
	public class FakeCommonAssemblyBuilder {
		public readonly Mock<CommonAssembly> CommonAssemblyMock;
		public readonly Mock<ModuleDefinition> MainMonoCecilModuleMock;
		public readonly Mock<AssemblyDefinition> MainMonoCecilAssemblyMock;

		public CommonAssembly CommonAssembly => CommonAssemblyMock.Object;
		public ModuleDefinition MainMonoCecilModule => MainMonoCecilModuleMock.Object;
		public AssemblyDefinition MainMonoCecilAssembly => MainMonoCecilAssemblyMock.Object;

		private readonly List<CommonType> commonTypes = new List<CommonType>();
		private readonly List<CommonType> commonTypesFromThisAssembly = new List<CommonType>();

		private FakeCommonAssemblyBuilder(Mock<CommonAssembly> commonAssemblyMock, Mock<ModuleDefinition> mainMonoCecilModuleMock, Mock<AssemblyDefinition> mainMonoCecilAssemblyMock) {
			CommonAssemblyMock = commonAssemblyMock;
			MainMonoCecilModuleMock = mainMonoCecilModuleMock;
			MainMonoCecilAssemblyMock = mainMonoCecilAssemblyMock;

			commonAssemblyMock.Setup(assembly => assembly.TypeFullNameToType).Returns(() => commonTypes.GroupBy(type => type.FullName).ToDictionary(group => group.Key, group => group.ToArray()));
			commonAssemblyMock.Setup(assembly => assembly.TypeReflectionToType).Returns(() => commonTypes.GroupBy(type => type.Reflection).ToDictionary(group => group.Key, group => group.ToArray()));
			commonAssemblyMock.Setup(assembly => assembly.Types).Returns(() => commonTypes.ToArray());
			commonAssemblyMock.Setup(assembly => assembly.TypesFromThisAssembly).Returns(() => commonTypesFromThisAssembly.ToArray());
			commonAssemblyMock.Setup(assembly => assembly.LoadInternal());
		}

		public static FakeCommonAssemblyBuilder Create() {
			var monoCecilModule = new Mock<ModuleDefinition> { CallBase = true };

			var monoCecilAssembly = new Mock<AssemblyDefinition> { CallBase = true };
			monoCecilAssembly.Setup(assembly => assembly.MainModule).Returns(() => monoCecilModule.Object);

			var commonAssembly = new Mock<CommonAssembly>(null, null, monoCecilAssembly.Object, null, false) { CallBase = true };
			return new FakeCommonAssemblyBuilder(commonAssembly, monoCecilModule, monoCecilAssembly);
		}

		public FakeCommonAssemblyBuilder AddCommonTypes(IEnumerable<CommonType> types, bool fromThisAssembly = true) {
			return types.Aggregate(this, (typeBuilder, type) => typeBuilder.AddCommonType(type, fromThisAssembly));
		}
		public FakeCommonAssemblyBuilder AddCommonType(CommonType commonType, bool fromThisAssembly = true) {
			if (fromThisAssembly) {
				commonTypesFromThisAssembly.Add(commonType);
				FakeCommonTypeBuilder.GetMockFor(commonType.MonoCecil).Setup(type => type.Module).Returns(() => MainMonoCecilModule);
			}

			commonTypes.Add(commonType);
			return this;
		}
	}
}