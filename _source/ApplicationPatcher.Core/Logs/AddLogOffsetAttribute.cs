using System;

namespace ApplicationPatcher.Core.Logs {
	[AttributeUsage(AttributeTargets.Method)]
	public class AddLogOffsetAttribute : Attribute {
	}
}