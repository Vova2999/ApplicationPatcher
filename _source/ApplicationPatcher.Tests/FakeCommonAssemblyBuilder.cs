﻿using System.Collections.Generic;
using ApplicationPatcher.Core.Types.Common;
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

		private FakeCommonAssemblyBuilder(Mock<CommonAssembly> commonAssemblyMock, Mock<ModuleDefinition> mainMonoCecilModuleMock, Mock<AssemblyDefinition> mainMonoCecilAssemblyMock) {
			CommonAssemblyMock = commonAssemblyMock;
			MainMonoCecilModuleMock = mainMonoCecilModuleMock;
			MainMonoCecilAssemblyMock = mainMonoCecilAssemblyMock;

			commonAssemblyMock.Setup(assembly => assembly.Types).Returns(() => commonTypes.ToArray());
		}

		public static FakeCommonAssemblyBuilder Create() {
			var monoCecilModule = new Mock<ModuleDefinition> { CallBase = true };

			var monoCecilAssembly = new Mock<AssemblyDefinition> { CallBase = true };
			monoCecilAssembly.Setup(assembly => assembly.MainModule).Returns(() => monoCecilModule.Object);

			var commonAssembly = new Mock<CommonAssembly>(null, null, monoCecilAssembly.Object, null, false) { CallBase = true };
			return new FakeCommonAssemblyBuilder(commonAssembly, monoCecilModule, monoCecilAssembly);
		}

		public FakeCommonAssemblyBuilder AddCommonType(CommonType commonType, bool fromThisAssembly = true) {
			if (fromThisAssembly)
				FakeCommonTypeBuilder.GetMockFor(commonType.MonoCecil).Setup(type => type.Module).Returns(() => MainMonoCecilModule);

			commonTypes.Add(commonType);
			return this;
		}
	}
}