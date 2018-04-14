using System;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;
using Ninject;

namespace ApplicationPatcher.Self {
	public static class Program {
		private static readonly Log log = Log.For(typeof(Program));

		[DoNotAddLogOffset]
		public static void Main() {
			try {
				Run();
			}
			catch (Exception exception) {
				log.Fatal(exception);
				throw;
			}
		}

		[DoNotAddLogOffset, UsedImplicitly]
		public static void Run() {
			var container = new StandardKernel(new ApplicationPatcherSelfNinjectModule());
			container.Get<ApplicationPatcherSelfProcessor>().PatchSelfApplication();
		}
	}
}