﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonAssembly : CommonBase<CommonAssembly>, IHasTypes {
		public CommonType[] Types { get; private set; }
		public readonly Assembly[] ReflectionAssembly;
		public readonly AssemblyDefinition MonoCecilAssembly;

		public CommonAssembly(Assembly[] reflectionAssembly, AssemblyDefinition monoCecilAssembly) {
			ReflectionAssembly = reflectionAssembly;
			MonoCecilAssembly = monoCecilAssembly;
		}

		protected override void LoadInternal() {
			base.LoadInternal();
			Types = CommonHelper.JoinTypes(GetAllReflectionTypes(), GetAllMonoCecilTypes());
		}

		private IEnumerable<Type> GetAllReflectionTypes() {
			return ReflectionAssembly.SelectMany(reflectionAssembly => reflectionAssembly.GetTypes());
		}

		private IEnumerable<TypeDefinition> GetAllMonoCecilTypes() {
			var types = new List<TypeDefinition>();
			MonoCecilAssembly.MainModule.Types.ForEach(type => AddType(types, type));

			return types;
		}
		private static void AddType(List<TypeDefinition> types, TypeDefinition currentType) {
			while (currentType != null && !types.Contains(currentType)) {
				types.Add(currentType);
				currentType = currentType.BaseType?.Resolve();
			}
		}
	}
}