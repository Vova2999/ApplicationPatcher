using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types {
	[PublicAPI]
	public interface ICommonMember<out TCommonMember, out TMonoCecil, out TReflection>
		where TCommonMember : ICommonMember<TCommonMember, TMonoCecil, TReflection> {
		string Name { get; }
		string FullName { get; }

		TMonoCecil MonoCecil { get; }
		TReflection Reflection { get; }

		TCommonMember Load(int depth = 0);
	}
}