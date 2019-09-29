using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Core.Factories {
	public interface ICommonAssemblyFactory {
		ICommonAssembly Create(string assemblyPath);
		void Save(ICommonAssembly commonAssembly, string assemblyPath, string signaturePath = null);
	}
}