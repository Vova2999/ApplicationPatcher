using ApplicationPatcher.Core.Logs;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers {
	[TestFixture]
	public abstract class PatcherTestsBase {
		[OneTimeSetUp]
		public void OneTimeSetUp() {
			Log.DefaultOffset = -1;
		}

		[TearDown]
		public void TearDown() {
			FakeCommonTypeBuilder.ClearCreatedTypes();
		}

		protected static Mock<TObject> CreateMockFor<TObject>(params object[] constructorParametes) where TObject : class {
			return new Mock<TObject>(constructorParametes) { CallBase = true };
		}
	}
}