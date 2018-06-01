using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class BytesExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static string ToHexString(this IEnumerable<byte> bytes) {
			return bytes
				.Select(x => Convert.ToString(x, 16))
				.Aggregate(new StringBuilder(),
					(builder, s) => {
						if (s.Length < 2)
							builder.Append("0");
						return builder.Append(s);
					})
				.ToString();
		}
	}
}