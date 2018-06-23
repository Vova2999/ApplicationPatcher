﻿using System.Linq;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Self;
using ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers;
using Moq;
using NUnit.Framework;

namespace ApplicationPatcher.Tests.Unit.Patchers.LoadedAssemblyPatchers {
	[TestFixture]
	public class MonoCecilConstsTypePatcherTests : PatcherTestsBase {
		private FakeCommonAssemblyBuilder assembly;
		private CommonType[] otherTypes;

		[SetUp]
		public void SetUp() {
			const int otherTypesCount = 3;

			otherTypes = Enumerable.Range(0, otherTypesCount).Select(x => FakeCommonTypeBuilder.Create($"OtherType{x}").Build()).ToArray();
			assembly = FakeCommonAssemblyBuilder.Create().AddCommonTypes(otherTypes);
		}

		[TearDown]
		public void CheckOtherTypes() {
			otherTypes.Select(FakeCommonTypeBuilder.GetMockFor).ForEach(otherTypeMock => otherTypeMock.Verify(mock => mock.LoadInternal(), Times.Never));
		}

		[Test]
		public void HaveConstsType() {
			var constsType = FakeCommonTypeBuilder.Create("Consts").AddField("PublicKey", typeof(string)).Build();

			Patch(constsType, new byte[] { 1, 2, 3, 202 });
			FakeCommonTypeBuilder.GetMockFor(constsType.GetField("PublicKey").MonoCecil).VerifySet(field => field.Constant = "010203ca", Times.Once);
			FakeCommonTypeBuilder.GetMockFor(constsType.GetField("PublicKey").MonoCecil).VerifySet(field => field.Constant = It.IsAny<object>(), Times.Once);
		}

		[Test]
		public void NotHaveConstsType() {
			Patch(null, null);
		}

		private void Patch(CommonType type, byte[] monoCecilNewPublicKey) {
			if (type != null)
				assembly.AddCommonType(type);

			new MonoCecilConstsTypePatcher(new ApplicationPatcherSelfConfiguration {
				MonoCecilNewPublicKey = monoCecilNewPublicKey
			}).Patch(assembly.CommonAssembly);
		}
	}
}