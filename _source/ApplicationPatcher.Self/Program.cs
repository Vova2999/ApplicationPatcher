using System;
using ApplicationPatcher.Core.Logs;
using JetBrains.Annotations;
using Ninject;

namespace ApplicationPatcher.Self {
	public static class Program {
		private static readonly ILog log = Log.For(typeof(Program));

		public static void Main() {
			try {
				Run();
			}
			catch (Exception exception) {
				log.Fatal(exception);
				throw;
			}
		}

		[UsedImplicitly]
		public static void Run() {
			var container = new StandardKernel(new ApplicationPatcherSelfNinjectModule());
			container.Get<ApplicationPatcherSelfProcessor>().PatchSelfApplication();
		}
	}
}