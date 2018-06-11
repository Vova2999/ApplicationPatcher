using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.LoadedAssemblyPatchers {
	[TestFixture]
	public class MonoCecilConstsTypePatcherTests : PatcherTestsBase {
		[Test]
		public void A() {
			var constsType = FakeCommonTypeBuilder.Create("Consts").AddField("PublicKey", typeof(string)).Build();
			var assembly = FakeCommonAssemblyBuilder.Create().AddCommonType(constsType);

			var applicationPatcherSelfConfiguration = new ApplicationPatcherSelfConfiguration { MonoCecilNewPublicKey = new byte[] { 1, 2, 3 } };
			new MonoCecilConstsTypePatcher(applicationPatcherSelfConfiguration).Patch(assembly.CommonAssembly);

			var publicKeyMonoCecilField = FakeCommonTypeBuilder.GetMockFor(constsType.GetField("PublicKey").MonoCecil);
			publicKeyMonoCecilField.VerifySet(field => field.Constant = "010203", Times.Once);
		}
	}
}