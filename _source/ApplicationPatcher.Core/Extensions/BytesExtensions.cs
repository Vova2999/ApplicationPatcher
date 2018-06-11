using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationPatcher.Core.Extensions {
	public static class BytesExtensions {
		public static string ToHexString(this IEnumerable<byte> bytes) {
			return bytes
				?.Select(x => Convert.ToString(x, 16))
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