using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class BytesExtensions {
		public static string ToHexString(this IEnumerable<byte> bytes) {
			return bytes?.Select(x => Convert.ToString(x, 16)).Aggregate(new StringBuilder(), (builder, s) => builder.Append(s.Length < 2 ? $"0{s}" : s)).ToString();
		}
	}
}