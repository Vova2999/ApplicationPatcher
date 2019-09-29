using ApplicationPatcher.Core.Factories;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfNinjectModule : NinjectModule {
		public override void Load() {
			Kernel?.Bind<ICommonAssemblyFactory>().ToMethod(c => new CommonAssemblyFactory());
			Kernel.Bind(c => c.FromThisAssembly().SelectAllClasses().BindAllInterfaces().Configure(y => y.InSingletonScope()));
			Kernel.Bind(c => c.FromThisAssembly().SelectAllClasses().BindAllBaseClasses().Configure(y => y.InSingletonScope()));
			Kernel?.Rebind<ApplicationPatcherSelfConfiguration>().ToMethod(c => ApplicationPatcherSelfConfiguration.ReadConfiguration());
		}
	}
}