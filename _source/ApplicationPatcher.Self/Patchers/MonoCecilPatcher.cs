using System;
using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Self.Patchers {
	[UsedImplicitly]
	public class MonoCecilPatcher : IPatcher {
		private static readonly Type[] selectedPatchingTypes = {
			typeof(AssemblyDefinition),
			typeof(CustomAttribute),
			typeof(FieldDefinition),
			typeof(MethodDefinition),
			typeof(MethodReference),
			typeof(ModuleDefinition),
			typeof(ParameterDefinition),
			typeof(PropertyDefinition),
			typeof(TypeDefinition),
			typeof(TypeReference)
		};

		private readonly Log log;

		public MonoCecilPatcher() {
			log = Log.For(this);
		}

		[DoNotAddLogOffset]
		public void Patch(CommonAssembly assembly) {
			log.Info("Patching selected types...");
			var foundedPatchingTypes = assembly.TypesFromThisAssembly.Where(type => selectedPatchingTypes.Any(type.Is)).ToArray();

			if (!foundedPatchingTypes.Any()) {
				log.Info("Not found selected types");
				return;
			}

			log.Debug("Selected types found:", foundedPatchingTypes.Select(viewModel => viewModel.FullName).OrderBy(fullName => fullName));

			foundedPatchingTypes.ForEach(type => type.MonoCecilType.IsSealed = false);
			log.Info("Sealed types was patched");
		}
	}
}