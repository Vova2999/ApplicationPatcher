using System;
using System.IO;
using System.Linq;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Factories;
using NUnit.Framework;

namespace ApplicationPatcher.Tests {
	[TestFixture]
	public class Class1 {
		[Test]
		public void A() {
			//new ApplicationPatcherProcessor(new CommonAssemblyFactory(), new ILoadedAssemblyPatcher[0], new INotLoadedAssemblyPatcher[0]).PatchApplication(@"C:\Users\Vladimir\Documents\Visual Studio 2017\Projects\WpfApp3\WpfApp3\bin\Debug\WpfApp3.exe");
			var commonAssembly = new CommonAssemblyFactory().Create(@"C:\Users\Vladimir\Documents\Visual Studio 2017\Projects\WpfApp3\WpfApp3\bin\Debug\WpfApp3.exe");
			commonAssembly.Load().TypesFromThisAssembly.Select(type => type.FullName).ForEach(Console.WriteLine);
		}
	}
}