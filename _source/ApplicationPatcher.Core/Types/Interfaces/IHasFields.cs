using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasFields : ICommonMember<IHasFields> {
		CommonField[] Fields { get; }

		Dictionary<string, CommonField[]> FieldNameToField { get; }
	}
}