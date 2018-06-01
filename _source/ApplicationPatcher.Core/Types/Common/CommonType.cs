using System;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Types.Common {
	public class CommonType : CommonBase<CommonType, Type, TypeDefinition>, IHasAttributes, IHasConstructors, IHasFields, IHasMethods, IHasProperties, IHasType {
		public Type Type => GetOrCreate(() => Reflection);
		public override string Name => GetOrCreate(() => MonoCecil.Name);
		public override string FullName => GetOrCreate(() => MonoCecil.FullName);
		public CommonAttribute[] Attributes { get; private set; }
		public CommonConstructor[] Constructors { get; private set; }
		public CommonField[] Fields { get; private set; }
		public CommonMethod[] Methods { get; private set; }
		public CommonProperty[] Properties { get; private set; }

		public CommonType(Type reflectionType, TypeDefinition monoCecilType) : base(reflectionType, monoCecilType) {
		}

		protected override void LoadInternal() {
			base.LoadInternal();

			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			Attributes = CommonHelper.JoinAttributes(Reflection.GetCustomAttributesData(), MonoCecil.CustomAttributes);
			Constructors = CommonHelper.JoinConstructors(Reflection.GetConstructors(bindingFlags), MonoCecil.Methods);
			Fields = CommonHelper.JoinFields(Reflection.GetFields(bindingFlags), MonoCecil.Fields);
			Methods = CommonHelper.JoinMethods(Reflection.GetMethods(bindingFlags), MonoCecil.Methods);
			Properties = CommonHelper.JoinProperties(Reflection.GetProperties(bindingFlags), MonoCecil.Properties);
		}
	}
}