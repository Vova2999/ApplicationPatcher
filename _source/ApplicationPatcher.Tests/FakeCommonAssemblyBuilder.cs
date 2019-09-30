using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;
using Mono.Cecil;
using Moq;

namespace ApplicationPatcher.Tests {
	[PublicAPI]
	public class FakeCommonAssemblyBuilder {
		public static FakeCommonAssemblyBuilder Create() {
			var monoCecilModule = new Mock<ModuleDefinition> { CallBase = true };

			var monoCecilAssembly = new Mock<AssemblyDefinition> { CallBase = true };
			monoCecilAssembly.Setup(assembly => assembly.MainModule).Returns(() => monoCecilModule.Object);

			var commonAssembly = new Mock<ICommonAssembly>();
			commonAssembly.Setup(assembly => assembly.MonoCecil).Returns(() => monoCecilAssembly.Object);
			return new FakeCommonAssemblyBuilder(commonAssembly, monoCecilModule, monoCecilAssembly);
		}

		public readonly Mock<ICommonAssembly> CommonAssemblyMock;
		public readonly Mock<ModuleDefinition> MainMonoCecilModuleMock;
		public readonly Mock<AssemblyDefinition> MainMonoCecilAssemblyMock;

		public ICommonAssembly CommonAssembly => CommonAssemblyMock.Object;
		public ModuleDefinition MainMonoCecilModule => MainMonoCecilModuleMock.Object;
		public AssemblyDefinition MainMonoCecilAssembly => MainMonoCecilAssemblyMock.Object;

		private readonly List<ICommonType> commonTypes = new List<ICommonType>();
		private readonly List<ICommonType> commonTypesFromThisAssembly = new List<ICommonType>();
		private readonly List<ICommonAttribute> commonAttributes = new List<ICommonAttribute>();

		private FakeCommonAssemblyBuilder(Mock<ICommonAssembly> commonAssemblyMock, Mock<ModuleDefinition> mainMonoCecilModuleMock, Mock<AssemblyDefinition> mainMonoCecilAssemblyMock) {
			CommonAssemblyMock = commonAssemblyMock;
			MainMonoCecilModuleMock = mainMonoCecilModuleMock;
			MainMonoCecilAssemblyMock = mainMonoCecilAssemblyMock;

			commonAssemblyMock.Setup(assembly => assembly.Types).Returns(() => commonTypes.ToArray());
			commonAssemblyMock.Setup(assembly => assembly.TypesFromThisAssembly).Returns(() => commonTypesFromThisAssembly.ToArray());
			commonAssemblyMock.Setup(assembly => assembly.Load(It.IsAny<int>())).Returns(() => commonAssemblyMock.Object);

			commonAssemblyMock.Setup(assembly => assembly.TypeTypeToTypes).Returns(() => commonTypes.GroupBy(type => type.Type).ToDictionary(group => group.Key, group => group.ToArray()));
			commonAssemblyMock.Setup(assembly => assembly.TypeFullNameToTypes).Returns(() => commonTypes.GroupBy(type => type.FullName).ToDictionary(group => group.Key, group => group.ToArray()));
			commonAssemblyMock.Setup(assembly => assembly.TypeTypeToAttributes).Returns(() => commonAttributes.GroupBy(attribute => attribute.Type).ToDictionary(group => group.Key, group => group.ToArray()));
			commonAssemblyMock.Setup(assembly => assembly.TypeFullNameToAttributes).Returns(() => commonAttributes.GroupBy(attribute => attribute.FullName).ToDictionary(group => group.Key, group => group.ToArray()));
		}

		public FakeCommonAssemblyBuilder AddCommonAttributes(IEnumerable<ICommonAttribute> attributes) {
			commonAttributes.AddRange(attributes);
			return this;
		}
		public FakeCommonAssemblyBuilder AddCommonAttribute(ICommonAttribute attribute) {
			commonAttributes.Add(attribute);
			return this;
		}

		public FakeCommonAssemblyBuilder AddCommonTypes(IEnumerable<ICommonType> types, bool fromThisAssembly = true) {
			return types.Aggregate(this, (typeBuilder, type) => typeBuilder.AddCommonType(type, fromThisAssembly));
		}
		public FakeCommonAssemblyBuilder AddCommonType(ICommonType commonType, bool fromThisAssembly = true) {
			if (fromThisAssembly) {
				commonTypesFromThisAssembly.Add(commonType);
				FakeCommonTypeBuilder.GetMockFor(commonType.MonoCecil).Setup(type => type.Module).Returns(() => MainMonoCecilModule);
			}

			commonTypes.Add(commonType);
			return this;
		}
	}
}