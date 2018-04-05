using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAssembly : CommonBase<CommonAssembly>, IHasTypes {
		public override string Name => GetOrCreate(() => MonoCecilAssembly.FullName);
		public override string FullName => GetOrCreate(() => MonoCecilAssembly.FullName);
		public CommonType[] Types { get; private set; }

		[UsedImplicitly]
		public CommonType[] TypesFromThisAssembly => GetOrCreate(() => Types.WhereFrom(this).ToArray());
		public readonly bool HaveSymbolStore;

		[UsedImplicitly]
		public readonly Assembly[] ReflectionAssembly;
		[UsedImplicitly]
		public readonly AssemblyDefinition MonoCecilAssembly;

		public CommonAssembly(bool haveSymbolStore, Assembly[] reflectionAssembly, AssemblyDefinition monoCecilAssembly) {
			HaveSymbolStore = haveSymbolStore;
			ReflectionAssembly = reflectionAssembly;
			MonoCecilAssembly = monoCecilAssembly;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Types = CommonHelper.JoinTypes(GetReflectionTypes(), GetMonoCecilTypes());
		}

		private IEnumerable<Type> GetReflectionTypes() {
			var types = new List<Type>();
			ReflectionAssembly.First().GetTypes().ForEach(type => AddType(types, type, t => t.BaseType));
			return types;
		}

		private IEnumerable<TypeDefinition> GetMonoCecilTypes() {
			var types = new List<TypeDefinition>();
			MonoCecilAssembly.MainModule.Types.ForEach(type => AddType(types, type, t => t.BaseType?.Resolve()));
			return types;
		}

		private static void AddType<TType>(List<TType> types, TType currentType, Func<TType, TType> getBaseType) {
			while (currentType != null && !types.Contains(currentType)) {
				types.Add(currentType);
				currentType = getBaseType(currentType);
			}
		}
	}
}