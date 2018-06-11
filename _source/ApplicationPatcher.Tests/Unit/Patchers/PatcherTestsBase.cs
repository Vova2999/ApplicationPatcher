using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers {
	[TestFixture]
	public abstract class PatcherTestsBase {
		[TearDown]
		public void TearDown() {
			FakeCommonTypeBuilder.ClearCreatedTypes();
		}

		protected static Mock<TObject> CreateMockFor<TObject>(params object[] constructorParametes) where TObject : class {
			return new Mock<TObject>(constructorParametes) { CallBase = true };
		}
	}
}