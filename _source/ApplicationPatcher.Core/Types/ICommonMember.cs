namespace ApplicationPatcher.Core.Types {
	public interface ICommonMember<out TCommonMember> {
		TCommonMember Load();
	}
}