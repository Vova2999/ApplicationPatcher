﻿using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonType : CommonBase<CommonType>, IHasFields, IHasMethods, IHasProperties, IHasAttributes {
		public CommonField[] Fields { get; private set; }
		public CommonMethod[] Methods { get; private set; }
		public CommonProperty[] Properties { get; private set; }
		public CommonAttribute[] Attributes { get; private set; }
		public readonly Type ReflectionType;
		public readonly TypeDefinition MonoCecilType;

		public CommonType(Type reflectionType, TypeDefinition monoCecilType) {
			ReflectionType = reflectionType;
			MonoCecilType = monoCecilType;
		}

		protected override void LoadInternal() {
			base.LoadInternal();

			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			Fields = CommonHelper.JoinFields(ReflectionType.GetFields(bindingFlags), MonoCecilType.Fields);
			Methods = CommonHelper.JoinMethods(ReflectionType.GetMethods(bindingFlags), MonoCecilType.Methods);
			Properties = CommonHelper.JoinProperties(ReflectionType.GetProperties(bindingFlags), MonoCecilType.Properties);
			Attributes = CommonHelper.JoinAttributes(ReflectionType.GetCustomAttributes(), MonoCecilType.CustomAttributes);
		}
	}
}