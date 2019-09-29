using System;
using ApplicationPatcher.Core.Logs;
using JetBrains.Annotations;
using Ninject;

namespace ApplicationPatcher.Self {
	public static class Program {
		private static readonly ILog Log = Core.Logs.Log.For(typeof(Program));

		public static void Main() {
			try {
				Run();
			}
			catch (Exception exception) {
				Log.Fatal(exception);
				throw;
			}
		}

		[UsedImplicitly]
		public static void Run() {
			var container = new StandardKernel(new ApplicationPatcherSelfNinjectModule());
			container.Get<ApplicationPatcherSelf>().PatchSelfApplication();
		}
	}
}