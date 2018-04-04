using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.Base {
	public abstract class CommonBase<TCommon> where TCommon : CommonBase<TCommon> {
		[UsedImplicitly]
		public abstract string Name { get; }
		[UsedImplicitly]
		public abstract string FullName { get; }

		private bool isLoaded;
		private readonly ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>();

		public TCommon Load() {
			var common = (TCommon)this;

			if (isLoaded)
				return common;

			LoadInternal();
			return common;
		}

		protected virtual void LoadInternal() {
			isLoaded = true;
		}

		protected TValue GetOrCreate<TValue>(Func<TValue> value) {
			return (TValue)values.GetOrAdd(new StackTrace().GetFrame(1).GetMethod().Name, _ => value());
		}
	}
}